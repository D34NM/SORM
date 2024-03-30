using System.Linq.Expressions;

namespace SORM.Core.Objects.Internal;

internal class LogicalOperator(BinaryExpression binaryExpression) : WhereExpression
{
    public override string ToString()
    {
        return binaryExpression.NodeType switch
        {
            ExpressionType.AndAlso or ExpressionType.And => "AND",
            ExpressionType.OrElse or ExpressionType.Or => "OR",
            _ => throw new NotSupportedException($"Logical operator {binaryExpression.NodeType} is not supported"),
        };
    }
}
