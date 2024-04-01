using System.Linq.Expressions;

namespace SORM.Core.Objects.Internal;

internal class MethodExpression(MethodCallExpression methodCallExpression) : WhereExpression
{
	public override string ToString()
    {
        if (methodCallExpression.Method.DeclaringType == typeof(string))
        {
            return methodCallExpression.Method.Name switch
            {
                nameof(string.Contains) => new ContainsExpression(methodCallExpression).ToString(),
                nameof(string.StartsWith) => new StartsWithExpression(methodCallExpression).ToString(),
                nameof(string.EndsWith) => new EndsWithExpression(methodCallExpression).ToString(),
                _ => throw new NotSupportedException($"Method {methodCallExpression.Method.Name} is not supported")
            };
        }

        if (methodCallExpression.Method.DeclaringType == typeof(Enumerable))
        {
            return methodCallExpression.Method.Name switch
            {
                nameof(Enumerable.Contains) => new ContainsExpression(methodCallExpression).ToString(),
                _ => throw new NotSupportedException($"Method {methodCallExpression.Method.Name} is not supported")
            };
        }

        throw new NotSupportedException($"Method {methodCallExpression.Method.Name} is not supported");
    }
}
