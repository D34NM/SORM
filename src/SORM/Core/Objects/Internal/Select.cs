using System.Reflection;

namespace SORM.Core.Objects.Internal;

internal class Select<T> where T : SalesforceEntity
{
    private readonly List<Column> _columns = [];

    public Select()
    {
        var properties = typeof(T).GetProperties();

        foreach (var property in properties.Where(p => p.DeclaringType == typeof(SalesforceEntity)))
        {
            if (property.IsResponseOnly())
            {
                continue;
            }

            _columns.Add(new Column(property));
        }
            
        foreach (var property in properties.Where(p => p.DeclaringType == typeof(T)))
        {
            if (property.IsResponseOnly())
            {
                continue;
            }

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
