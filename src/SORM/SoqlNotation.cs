namespace SORM;

internal static class SoqlNotation
{
    public static string SELECT(string fields) => $"SELECT {fields}";
    public static string FROM(string table) => $"FROM {table}";
    public static string WHERE(string condition) => $"WHERE {condition}";
    public static string LIMIT(int limit) => $"LIMIT {limit}";
    public static string OFFSET(int offset) => $"OFFSET {offset}";
    public static string ORDER_BY(string field) => $"ORDER BY {field}";
    public static string ORDER_BY_ASC(string field) => $"ORDER BY {field} ASC";
    public static string ORDER_BY_DESC(string field) => $"ORDER BY {field} DESC";
}