using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace SORM;

internal class Where<T> where T : class
{
    private readonly BinaryFieldExpression _binaryFieldExpression;

    public Where(Expression<Func<T, bool>> expression)
    {
        if (expression.Body is BinaryExpression binaryExpression)
        {
            _binaryFieldExpression = new BinaryFieldExpression(binaryExpression);
        }
        else
        {
            throw new NotSupportedException($"Expression type {expression.Body.GetType()} is not supported");
        }
    }

    public override string ToString()
    {
        return $"WHERE {_binaryFieldExpression}";
    }
}

internal class WhereExpression { }

internal class BinaryFieldExpression : WhereExpression
{
    private readonly List<WhereExpression> _expressions = [];

    public BinaryFieldExpression(BinaryExpression binaryExpression)
    {
        var (left, right) = (binaryExpression.Left, binaryExpression.Right);
        
        if (left is BinaryExpression && right is BinaryExpression)
        {
            _expressions.Add(new BinaryFieldExpression((BinaryExpression)left));
            _expressions.Add(new LogicalOperator(binaryExpression));
            _expressions.Add(new BinaryFieldExpression((BinaryExpression)right));
        }

        if (left is BinaryExpression && right is MemberExpression)
        {
            _expressions.Add(new BinaryFieldExpression((BinaryExpression)left));
            _expressions.Add(new LogicalOperator(binaryExpression));
            _expressions.Add(new FieldExpression(binaryExpression));
        }

        if (left is MemberExpression && right is BinaryExpression)
        {
            _expressions.Add(new FieldExpression(binaryExpression));
            _expressions.Add(new LogicalOperator(binaryExpression));
            _expressions.Add(new BinaryFieldExpression((BinaryExpression)right));
        }

        if (left is MemberExpression && right is ConstantExpression)
        {
            _expressions.Add(new FieldExpression(binaryExpression));
        }
    }

    public override string ToString()
    {
        return string.Join(" ", _expressions);
    }
}

internal class FieldExpression : WhereExpression
{
    private readonly MemberExpression _memberExpression;
    private readonly ComparisonOperator _comparisonOperator;
    private readonly ConstantExpression _constantExpression;

    public FieldExpression(BinaryExpression binaryExpression)
    {
        if (binaryExpression is
            {
                Left: MemberExpression memberExpression,
                Right: ConstantExpression constantExpression
            })
        {
            _memberExpression = memberExpression;
            _comparisonOperator = new ComparisonOperator(binaryExpression);
            _constantExpression = constantExpression;
        }
        else
        {
            throw new NotSupportedException($"Expression type {binaryExpression.Left.GetType()} is not supported");
        }
    }

    public FieldExpression(MemberExpression memberExpression)
    {
        _memberExpression = memberExpression;
        _comparisonOperator = new ComparisonOperator(ExpressionType.Equal);
        _constantExpression = Expression.Constant(true);
    }

    public override string ToString()
    {

        var name = _memberExpression.Member.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? _memberExpression.Member.Name;

        if (_constantExpression.Type == typeof(string))
        {
            return $"{name} {_comparisonOperator} '{_constantExpression.Value}'";
        }
        
        return $"{name} {_comparisonOperator} {_constantExpression.Value}";
    }
}

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
            _ => throw new NotSupportedException($"Comparison operator {_expressionType} is not supported"),
        };
    }
}

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