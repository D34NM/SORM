using System.Linq.Expressions;

namespace SORM.Core.Objects.Internal.SOQL.Expressions;

internal class ArrayExpression : MethodExpression
{
    private readonly MemberExpression _column;
    private readonly NewArrayExpression _value;
    
    public ArrayExpression(MethodCallExpression methodCallExpression) : base(methodCallExpression)
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
    }

	public override string ToString()
    {
        var first = _value.Expressions.FirstOrDefault() ?? throw new NotSupportedException($"No values provided.");

		var array = string.Join(",", _value.Expressions.Select(exp => 
        {
			if (exp is not ConstantExpression value)
			{
				throw new NotSupportedException($"Expression type {exp.GetType()} is not supported");
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
