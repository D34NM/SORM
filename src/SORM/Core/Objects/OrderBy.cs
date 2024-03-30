using System.Linq.Expressions;
using System.Reflection;

namespace SORM.Core.Objects;

/// <summary>
/// Represents an ORDER BY clause in a SOQL query. 
/// </summary>
/// <seealso cref="https://developer.salesforce.com/docs/atlas.en-us.soql_sosl.meta/soql_sosl/sforce_api_calls_sosl_select_order_by_with_limit.htm"/>
/// <typeparam name="T">The custom type used in the query.</typeparam>
public sealed class OrderBy<T> where T : class
{
    private readonly List<PropertyInfo> _properties = [];

    private Direction _direction = Direction.Ascending;
    private Nulls _nulls = Nulls.First;
    private Limit _limit = Limit.By(100);
    private Offset _offset = Offset.By(0);

    private string _clause = string.Empty;

    public void Column(
        Expression<Func<T, object>> by,
        Direction direction = Direction.Ascending,
        Nulls nulls = Nulls.First,
        Limit? limit = default,
        Offset? offset = default)
    {
        _direction = direction;
        _nulls = nulls;
        
        if (limit is not null)
        {
            _limit = limit;
        }

        if (offset is not null)
        {
            _offset = offset;
        }

        switch (by.Body)
        {
            case MemberExpression memberExpression:
                _properties.Add((PropertyInfo)memberExpression.Member);
                break;
            case UnaryExpression { Operand: MemberExpression memberExpression }:
                _properties.Add((PropertyInfo)memberExpression.Member);
                break;
            default:
                throw new NotSupportedException($"Expression type {by.Body.GetType()} is not supported");
        }
    }

    public void Columns(
        Direction direction = Direction.Ascending,
        Nulls nulls = Nulls.First,
        params Expression<Func<T, object>>[] expressions)
    {
        _direction = direction;
        _nulls = nulls;

        foreach (var expression in expressions)
        {
            Column(expression, direction, nulls);
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

        var direction = _direction == Direction.Ascending ? "ASC" : "DESC";
        var nulls = _nulls == Nulls.First ? "FIRST" : "LAST";

        _clause = $"ORDER BY {string.Join(",", columnNames)} {direction} NULLS {nulls} {_limit} {_offset}";

        return _clause;
    }
}
