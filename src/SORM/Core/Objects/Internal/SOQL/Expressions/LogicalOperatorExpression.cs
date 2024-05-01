using System.Linq.Expressions;

namespace SORM.Core.Objects.Internal.SOQL.Expressions;

internal class LogicalOperatorExpression(BinaryExpression binaryExpression) : WhereExpression
{
    private static readonly string _and = "AND";
    private static readonly string _or = "OR";

    public override string ToString()
    {
        return binaryExpression.NodeType switch
        {
            ExpressionType.AndAlso or ExpressionType.And => _and,
            ExpressionType.OrElse or ExpressionType.Or => _or,
            _ => throw new NotSupportedException($"Logical operator {binaryExpression.NodeType} is not supported"),
        };
    }
}
