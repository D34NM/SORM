using System.Reflection;
using System.Text.Json.Serialization;

namespace SORM.Core.Objects.Internal;

internal class SalesforceObjectDescriptor : PropertyDescriptor
{
    private readonly string _tableName;
    private readonly ObjectDescriptor _type;

    public SalesforceObjectDescriptor(PropertyInfo propertyInfo) : base(propertyInfo)
    {
        _tableName = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? $"{propertyInfo.Name}__r";
        _type = new ObjectDescriptor(propertyInfo.PropertyType);
    }

    public override bool IsSalesforceObject => true;

    public override Descriptor[] GetProperties()
    {
        return _type.GetProperties();
    }

    public override string ToString()
    {
        return _tableName;
    }
}
