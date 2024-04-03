using System.Text;

namespace SORM.Core.Objects.Internal;

internal class SalesforceObjectColumn : Column
{
    private readonly SalesforceObjectDescriptor _property;

    public SalesforceObjectColumn(PropertyDescriptor property)
    {
        _property = (SalesforceObjectDescriptor)property;
    }

    public SalesforceObjectColumn(Descriptor property)
    {
        _property = (SalesforceObjectDescriptor)property;
    }

    public override string ToString()
    {
        var properties = _property.GetProperties();

        List<Column> columns = [];

        foreach (var property in properties)
        {
            if (property.IsRelationship)
            {
                columns.Add(new RelationshipColumn(property));
                continue;
            }

            columns.Add(new FieldColumn(property));
        }

        var stringBuilder = new StringBuilder();

        for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
        {
            var column = columns[columnIndex];

            if (column is FieldColumn fieldColumn)
            {
                stringBuilder.Append($"{_property}.{fieldColumn}");
            }

            if (column is RelationshipColumn relationshipColumn)
            {
                stringBuilder.Append($"{relationshipColumn}");
            }

            if (columnIndex < columns.Count - 1)
            {
                stringBuilder.Append(',');
            }
        }

        return $"{stringBuilder}";
    }
}
