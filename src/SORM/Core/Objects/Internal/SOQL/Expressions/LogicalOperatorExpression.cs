using System.Linq.Expressions;
using SORM.Core.Objects.Internal.SOQL.Expressions;

namespace SORM.Core.Objects.Internal.SOQL.Expressions;

internal class LogicalOperatorExpression(BinaryExpression binaryExpression) : WhereExpression
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
