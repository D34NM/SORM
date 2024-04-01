using System.Reflection;
using System.Text.Json.Serialization;

namespace SORM.Core.Objects.Internal;

internal sealed class RelationshipDescriptor : Descriptor
{
    private readonly string _tableName;
    private readonly ObjectDescriptor _type;

	public RelationshipDescriptor(PropertyInfo propertyInfo)
    {
        _type = new ObjectDescriptor(propertyInfo.PropertyType.GetGenericArguments()[0]);
        _tableName = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? $"{_type.Name}__r";
	}

    public override bool IsRelationship => true;

    public override Descriptor[] GetProperties()
    {
        return _type.GetProperties();
    }

	public override string ToString()
    {
        return _tableName;
    }
}