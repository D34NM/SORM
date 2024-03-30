using System.Reflection;

namespace SORM.Core.Objects.Internal;

internal class RelationshipColumn : Column
{
    private readonly List<Column> _columns = [];

    public RelationshipColumn(PropertyInfo property) : base(property)
    {
        var type = property.PropertyType.GetGenericArguments()[0];
        var innerProperties = type.GetProperties();

         foreach (var innerProperty in innerProperties.Where(p => p.DeclaringType == typeof(SalesforceEntity)))
        {
            if (innerProperty.IsResponseOnly())
            {
                continue;
            }

            _columns.Add(new Column(innerProperty));
        }

        foreach (var innerProperty in innerProperties.Where(p => p.DeclaringType == type))
        {
            if (innerProperty.IsResponseOnly())
            {
                continue;
            }
            
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
