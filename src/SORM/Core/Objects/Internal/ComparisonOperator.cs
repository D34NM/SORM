using System.Linq.Expressions;

namespace SORM.Core.Objects.Internal;

internal class ComparisonOperator
{
    private readonly ExpressionType _expressionType;
    
    public ComparisonOperator(BinaryExpression binaryExpression)
    {
        _expressionType = binaryExpression.NodeType;
    }

    public ComparisonOperator(ExpressionType expressionType) 
    {
        _expressionType = expressionType;
    }

    public ComparisonOperator(MethodCallExpression expressionType)
    {
        if (expressionType.Method.Name != "Contains")
        {
            throw new NotSupportedException($"Method {expressionType.Method.Name} is not supported");
        }
        
        _expressionType = expressionType.NodeType;
    }

    public override string ToString()
    {
        return _expressionType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.Call => "IN",
            _ => throw new NotSupportedException($"Comparison operator {_expressionType} is not supported"),
        };
    }
}
