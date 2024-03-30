using System.Linq.Expressions;
using System.Reflection;

namespace SORM.Core.Objects;

public sealed class GroupBy<T> where T : class
{
    private readonly List<PropertyInfo> _properties = [];
    private string _clause = string.Empty;

    public void Column(Expression<Func<T, object>> expression)
    {
        switch (expression.Body)
        {
            case MemberExpression memberExpression:
                _properties.Add((PropertyInfo)memberExpression.Member);
                break;
            case UnaryExpression { Operand: MemberExpression memberExpression}:
                _properties.Add((PropertyInfo)memberExpression.Member);
                break;
            default:
                throw new NotSupportedException($"Expression type {expression.Body.GetType()} is not supported");
        }
    }

    public void Columns(params Expression<Func<T, object>>[] expressions)
    {
        foreach (var expression in expressions)
        {
            Column(expression);
        }
    }

    public override string ToString()
    {
        if (_properties.Count == 0)
        {
            return string.Empty;
        }

        if (!string.IsNullOrEmpty(_clause))
        {
            return _clause;
        }

        var columnNames = _properties.Select(p => 
        {
            var columnName = p.GetDecoratedOrPropertyName();
            return columnName;
        });

        _clause = $"GROUP BY {string.Join(",", columnNames)}";

        return _clause;
    }
}
