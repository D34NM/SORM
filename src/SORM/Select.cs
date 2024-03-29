using System.Reflection;
using System.Text.Json.Serialization;

namespace SORM;

internal class Select<T> where T : class
{
    private readonly List<Column> _columns = new();

    public Select()
    {
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(Relationship<>))
                {
                    _columns.Add(new RelationshipColumn(property));
                    continue;
                }
            }

            _columns.Add(new Column(property));
        }
    }

    public override string ToString()
    {
        return $"SELECT {string.Join(",", _columns.Select(p => p.ToString()))}";
    }
}

internal class Column
{
    private readonly PropertyInfo _property;
    private readonly string _name;

    public Column(PropertyInfo property)
    {
        _property = property;
        _name = _property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? _property.Name;
    }

    public override string ToString()
    {
        return _name;
    }
}

internal class RelationshipColumn : Column
{
    private readonly string _name;
    private readonly List<Column> _columns = new();

    public RelationshipColumn(PropertyInfo property) : base(property)
    {
        var type = property.PropertyType.GetGenericArguments()[0];
        var innerProperties = type.GetProperties();

        _name = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;

        foreach (var innerProperty in innerProperties)
        {
            if (innerProperty.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                if (innerProperty.PropertyType.IsGenericType &&
                    innerProperty.PropertyType.GetGenericTypeDefinition() == typeof(Relationship<>))
                {
                    throw new NotSupportedException("Nested relationships are not supported");
                }
            }

            _columns.Add(new Column(innerProperty));
        }
    }

    public override string ToString()
    {
        var columns = string.Join(",", _columns.Select(c => c.ToString()));

        return $"(SELECT {columns} FROM {_name})";
    }
}