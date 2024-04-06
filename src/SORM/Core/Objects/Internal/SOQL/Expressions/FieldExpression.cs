using System.Linq.Expressions;

namespace SORM.Core.Objects.Internal.SOQL.Expressions;

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
