namespace SORM.Core.Objects.Internal;

internal class FieldColumn : Column
{
	private readonly PropertyDescriptor _property;

	public FieldColumn(PropertyDescriptor property)
    {
		_property = property;
	}

    public FieldColumn(Descriptor property)
    {
        _property = (PropertyDescriptor)property;
    }

    public override string ToString()
    {
        return $"{_property.ColumnName}";
    }
}
