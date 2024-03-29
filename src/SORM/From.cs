using System.Reflection;
using SORM.DataAnnotations;

namespace SORM;

internal class From<T> where T : class
{
    private string _clause = string.Empty;

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(_clause))
        {
            return _clause;
        }

        var type = typeof(T);

        var tableAttribute = type.GetCustomAttribute<TableAttribute>();

        if (tableAttribute is null)
        {
            _clause = $"FROM {type.Name}";
            return _clause;
        }

        _clause = $"FROM {tableAttribute.Name}";

        return _clause;
    }
}