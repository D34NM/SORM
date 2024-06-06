using System.Reflection;
using System.Text.Json.Serialization;

namespace SORM;

internal static class MemberInfoExtensions
{
    public static string GetDecoratedOrPropertyName(this MemberInfo memberInfo)
    {
        var columnAttribute = memberInfo.GetCustomAttribute<JsonPropertyNameAttribute>();

        return columnAttribute?.Name ?? memberInfo.Name;
    }
}