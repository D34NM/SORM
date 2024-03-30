using System.Reflection;
using System.Text.Json.Serialization;
using SORM.DataAnnotations;

namespace SORM;

internal static class PropertyInfoExtensions
{
    public static string GetDecoratedOrPropertyName(this PropertyInfo propertyInfo)
    {
        var columnAttribute = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();

        return columnAttribute?.Name ?? propertyInfo.Name;
    }

    public static bool IsRelationship(this PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.IsClass &&
               propertyInfo.PropertyType != typeof(string) &&
               propertyInfo.PropertyType.IsGenericType &&
               propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Relationship<>);
    }

    public static PropertyInfo GetKeyProperty(this IEnumerable<PropertyInfo> propertyInfos)
    {
        return propertyInfos
            .Single(p => p.GetCustomAttribute<KeyAttribute>() != null);
    }
}