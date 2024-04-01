using System.Linq.Expressions;

namespace SORM.Core.Objects.Internal;

internal class EndsWithExpression : MethodExpression
{
    private readonly MemberExpression _column;
    private readonly ConstantExpression _value;

    public EndsWithExpression(MethodCallExpression methodCallExpression) : base(methodCallExpression)
    {
        if (methodCallExpression.Object is not MemberExpression memberExpression)
        {
            throw new NotSupportedException($"Expression type {methodCallExpression} is not supported");
        }

        _column = memberExpression;
        _value = (ConstantExpression)methodCallExpression.Arguments[0];
    }

    public override string ToString()
    {
        var name = _column.Member.GetDecoratedOrPropertyName();
        var value = _value.Value;

        return $"{name} LIKE '%{value}'";
    }
}
