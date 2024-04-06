namespace SORM.Core.Objects.Internal.SOQL;

internal class RelationshipColumn : Column
{
	private readonly RelationshipDescriptor _property;

	public RelationshipColumn(RelationshipDescriptor property)
    {
        _property = property;
    }

    public RelationshipColumn(Descriptor property)
    {
        _property = (RelationshipDescriptor)property;
    }

	public override string ToString()
    {
        var innerProperties = _property.GetProperties();

        List<Column> columns = [];

        foreach (var property in innerProperties)
        {
            if (property.IsRelationship)
            {
                columns.Add(new RelationshipColumn(property));
                continue;
            }

            columns.Add(new FieldColumn(property));
        }

        return $"(SELECT {string.Join(",", columns.Select(c => c.ToString()))} FROM {_property})";
    }
}
