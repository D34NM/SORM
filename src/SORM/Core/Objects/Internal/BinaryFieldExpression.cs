using System.Linq.Expressions;

namespace SORM.Core.Objects.Internal;

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
