using System.Linq.Expressions;
using System.Text;

namespace SORM;

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

        if (left is MethodCallExpression && right is BinaryExpression)
        {
            _expressions.Add(new MethodExpression((MethodCallExpression)left));
            _expressions.Add(new LogicalOperator(binaryExpression));
            _expressions.Add(new BinaryFieldExpression((BinaryExpression)right));
        }

        if (left is BinaryExpression && right is MethodCallExpression)
        {
            _expressions.Add(new BinaryFieldExpression((BinaryExpression)left));
            _expressions.Add(new LogicalOperator(binaryExpression));
            _expressions.Add(new MethodExpression((MethodCallExpression)right));
        }
    }

    public override string ToString()
    {
        return string.Join(" ", _expressions);
    }
}

internal class MethodExpression : WhereExpression
{
    private readonly MemberExpression _column;
    private readonly ComparisonOperator _operation;
    private readonly NewArrayExpression _value;
    

    public MethodExpression(MethodCallExpression methodCallExpression)
    {
        if (methodCallExpression.Arguments[1] is not MemberExpression column)
        {
            throw new NotSupportedException($"Expression type {methodCallExpression.Arguments[1].GetType()} is not supported");
        }

        if (methodCallExpression.Arguments[0] is not NewArrayExpression value)
        {
            throw new NotSupportedException($"Expression type {methodCallExpression.Arguments[0].GetType()} is not supported");
        }

        _column = column;
        _value = value;
        _operation = new ComparisonOperator(methodCallExpression);
    }

	public override string ToString()
    {
        var first = _value.Expressions.FirstOrDefault() ?? throw new NotSupportedException($"No values provided.");

		var array = string.Join(",", _value.Expressions.Select(v => 
        {
			if (v is not ConstantExpression value)
			{
				throw new NotSupportedException($"Expression type {v.GetType()} is not supported");
			}

			if (value.Type == typeof(string))
            {
                return $"'{value.Value}'";
            }
            
            return value.Value;
        }));

		var name = _column.Member.GetDecoratedOrPropertyName();

        return $"{name} IN ({string.Join(",", array)})";
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
        var name = _memberExpression.Member.GetDecoratedOrPropertyName();

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