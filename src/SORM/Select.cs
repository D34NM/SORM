using System.Reflection;

namespace SORM;

internal class Select<T> where T : class
{
    private readonly List<Column> _columns = [];

    public Select()
    {
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            if (property.IsRelationship())
            {
                _columns.Add(new RelationshipColumn(property));
                continue;
            }

            _columns.Add(new Column(property));
        }
    }

    public override string ToString()
    {
        return $"SELECT {string.Join(",", _columns.Select(c => c.ToString()))}";
    }
}

internal class Column
{
    private readonly PropertyInfo _property;
    private readonly string _name;

    public Column(PropertyInfo property)
    {
        _property = property;
        _name = _property.GetDecoratedOrPropertyName();
    }

    public override string ToString()
    {
        return _name;
    }
}

internal class RelationshipColumn : Column
{
    private readonly List<Column> _columns = [];

    public RelationshipColumn(PropertyInfo property) : base(property)
    {
        var type = property.PropertyType.GetGenericArguments()[0];
        var innerProperties = type.GetProperties();

        foreach (var innerProperty in innerProperties)
        {
            if (innerProperty.IsRelationship())
            {
                _columns.Add(new RelationshipColumn(innerProperty));
                continue;
            }

            _columns.Add(new Column(innerProperty));
        }
    }

    public override string ToString()
    {
        var columns = string.Join(",", _columns.Select(c => c.ToString()));

        return $"(SELECT {columns} FROM {base.ToString()})";
    }
}