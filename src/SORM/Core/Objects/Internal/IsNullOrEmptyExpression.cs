using System.Linq.Expressions;

namespace SORM.Core.Objects.Internal;

internal class IsNullOrEmptyExpression : MethodExpression
{
    private readonly MemberExpression _column;

    public IsNullOrEmptyExpression(MethodCallExpression methodCallExpression) : base(methodCallExpression)
    {
        if (methodCallExpression.Arguments.Single() is not MemberExpression memberExpression)
        {
            throw new NotSupportedException($"Expression type {methodCallExpression} is not supported");
        }

        _column = memberExpression;
    }

    public override string ToString()
    {
        var name = _column.Member.GetDecoratedOrPropertyName();

        return $"{name} IS NULL OR {name} = ''";
    }
}
