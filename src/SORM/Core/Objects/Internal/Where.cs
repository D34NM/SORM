using System.Linq.Expressions;

namespace SORM.Core.Objects.Internal;

internal class Where<T> where T : class
{
    private readonly WhereExpression _expression;

    public Where(Expression<Func<T, bool>> expression)
    {
        if (expression.Body is BinaryExpression binaryExpression)
        {
            _expression = new BinaryFieldExpression(binaryExpression);
        }
        else if (expression.Body is MethodCallExpression methodCallExpression)
        {
            _expression = new MethodExpression(methodCallExpression);
        }
        else
        {
            throw new NotSupportedException($"Expression type {expression.Body.GetType()} is not supported");
        }
    }

    public override string ToString()
    {
        return $"WHERE {_expression}";
    }
}
