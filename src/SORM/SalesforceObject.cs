using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SORM.DataAnnotations;

namespace SORM;

/// <summary>
/// Represents a Salesforce object.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SalesforceObject<T>
    where T : class
{
    private readonly Type _type;
    private readonly PropertyInfo[] _properties;

    public SalesforceObject()
    {
        _type = typeof(T);
        _properties = _type.GetProperties();
    }

    public string FindAsync(params object[] keys)
    {
        if (keys.Length == 0)
        {
            throw new ArgumentException("No keys provided.");
        }

        var stringBuilder = new StringBuilder();

        stringBuilder
            .Append(new Select<T>())
            .Append(' ')
            .Append(new From<T>());

        var keyProperty = _properties.GetKeyProperty();

        var expression = Expression.Lambda<Func<T, bool>>(
            Expression.Equal(
                Expression.Property(Expression.Parameter(_type), keyProperty),
                Expression.Constant(keys[0])),
            Expression.Parameter(_type));

        for (var i = 1; i < keys.Length; i++)
        {
            expression = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    expression.Body,
                    Expression.Equal(
                        Expression.Property(Expression.Parameter(_type), keyProperty),
                        Expression.Constant(keys[i]))),
                    Expression.Parameter(_type));
        }
        
        stringBuilder.Append(' ');
        stringBuilder.Append(new Where<T>(expression));

        return stringBuilder.ToString();
    }

    public string FindAllAsync(uint limit = 100)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder
            .Append(new Select<T>())
            .Append(' ')
            .Append(new From<T>())
            .Append(' ')
            .Append(Limit.By(limit));

        return stringBuilder.ToString();
    }

    public string FindAllAsync(Expression<Func<T, bool>> predicate, uint limit = 100)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(new Select<T>())
            .Append(' ')
            .Append(new From<T>())
            .Append(' ')
            .Append(new Where<T>(predicate))
            .Append(' ')
            .Append(Limit.By(limit));

        return stringBuilder.ToString();
    }

    public string FindAllAsync(
        Expression<Func<T, bool>> predicate,
        Action<OrderBy<T>> orderBy)
    {
        var orderByValue = new OrderBy<T>();
        orderBy(orderByValue);

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(new Select<T>())
            .Append(' ')
            .Append(new From<T>())
            .Append(' ')
            .Append(new Where<T>(predicate))
            .Append(' ')
            .Append(orderByValue);

        return stringBuilder.ToString();
    }
}
