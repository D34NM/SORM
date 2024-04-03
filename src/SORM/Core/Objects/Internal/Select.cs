namespace SORM.Core.Objects.Internal;

internal class Select
{
    private readonly List<Column> _columns = [];

    public Select(ObjectDescriptor type)
    {
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            if (property.IsRelationship)
            {
                _columns.Add(new RelationshipColumn(property));
                continue;
            }

            if (property.IsSalesforceObject)
            {
                _columns.Add(new SalesforceObjectColumn(property));
                continue;
            }

            _columns.Add(new FieldColumn(property));
        }
    }

    public override string ToString()
    {
        return $"SELECT {string.Join(",", _columns.Select(c => c.ToString()))}";
    }
}
