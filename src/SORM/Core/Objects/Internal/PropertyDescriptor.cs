using System.Reflection;
using System.Text.Json.Serialization;

namespace SORM.Core.Objects.Internal;

internal class PropertyDescriptor(PropertyInfo propertyInfo) : Descriptor
{
	public string Name { get; } = propertyInfo.Name;
	public string ColumnName { get; } = DetermineColumnName(propertyInfo);

    public PropertyInfo GetPropertyInfo()
    {
        return propertyInfo;
    }

	private static string DetermineColumnName(PropertyInfo propertyInfo)
    {
        var columnAttribute = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();

        return columnAttribute?.Name ?? propertyInfo.Name;
    }

    public override string ToString()
    {
        return ColumnName;
    }
}