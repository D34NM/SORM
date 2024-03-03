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
    private string _cachedQuery = string.Empty;

    public SalesforceObject()
    {
        _type = typeof(T);
        _properties = _type.GetProperties();
    }

    public Task<List<T>> FindAsync(params object[] keys)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"SELECT {ParseProperties()}");

        var tableAttribute = _type.GetCustomAttribute<TableAttribute>();

        if (tableAttribute is null)
        {
            stringBuilder.Append($" FROM {_type.Name}");
        }
        else
        {
            stringBuilder.Append($" FROM {tableAttribute.Name}");
        }

        stringBuilder.Append(" WHERE ");

        var keyProperty = _properties.SingleOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null) ?? throw new InvalidOperationException("No key attribute found.");
		
        var keyPropertyName =
             keyProperty.GetCustomAttribute<ColumnAttribute>()?.Name;

        if (string.IsNullOrWhiteSpace(keyPropertyName))
        {
            keyPropertyName = keyProperty.Name;
        }

        for (var i = 0; i < keys.Length; i++)
        {
            var key = keys[i];

            if (keyProperty.PropertyType == typeof(string))
            {
                stringBuilder.Append($"{keyProperty.Name} = '{key}'");
            }
            else
            {
                stringBuilder.Append($"{keyProperty.Name} = {key}");
            }

            if (i < keys.Length - 1)
            {
                stringBuilder.Append(" AND ");
            }
        }

        var query = stringBuilder.ToString();

        return Task.FromResult<List<T>>(null!);
    }

    public Task<List<T>> FindAllAsync(uint limit = 100)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"SELECT {ParseProperties()}");

        var tableAttribute = _type.GetCustomAttribute<TableAttribute>();

        if (tableAttribute is null)
        {
            stringBuilder.Append($" FROM {_type.Name}");
        }
        else
        {
            stringBuilder.Append($" FROM {tableAttribute.Name}");
        }

        stringBuilder.Append($" LIMIT {limit}");

        var query = stringBuilder.ToString();

        return Task.FromResult<List<T>>(null!);
    }

    public Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate, uint limit = 100)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"SELECT {ParseProperties()}");

        var tableAttribute = _type.GetCustomAttribute<TableAttribute>();

        if (tableAttribute is null)
        {
            stringBuilder.Append($" FROM {_type.Name}");
        }
        else
        {
            stringBuilder.Append($" FROM {tableAttribute.Name}");
        }

        stringBuilder.Append(" WHERE ");

        stringBuilder.Append(ParseExpression(predicate));

        stringBuilder.Append($" LIMIT {limit}");

        var query = stringBuilder.ToString();

        return Task.FromResult<List<T>>(null!);
    }

    public Task<List<T>> FindAllAsync(
        Expression<Func<T, bool>> predicate, 
        Expression<Func<T, object>> orderBy, 
        bool descending = false, 
        uint limit = 100)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"SELECT {ParseProperties()}");

        var tableAttribute = _type.GetCustomAttribute<TableAttribute>();

        if (tableAttribute is null)
        {
            stringBuilder.Append($" FROM {_type.Name}");
        }
        else
        {
            stringBuilder.Append($" FROM {tableAttribute.Name}");
        }

        stringBuilder.Append($" WHERE {ParseExpression(predicate)}");

        stringBuilder.Append($" ORDER BY {ParseOrderByExpression(orderBy)}");

        if (descending)
        {
            stringBuilder.Append(" DESC");
        }

        stringBuilder.Append($" LIMIT {limit}");

        var query = stringBuilder.ToString();

        return Task.FromResult<List<T>>(null!);
    }

    private string ParseOrderByExpression(Expression<Func<T, object>> orderBy)
    {
        var stringBuilder = new StringBuilder();

        if (orderBy.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException("Invalid expression.");
        }

        var property = memberExpression.Member as PropertyInfo;

        if (property is null)
        {
            throw new InvalidOperationException("Invalid expression.");
        }

        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

        if (columnAttribute is null)
        {
            stringBuilder.Append(property.Name);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(columnAttribute.Name))
            {
                stringBuilder.Append(property.Name);
            }
            else
            {
                stringBuilder.Append(columnAttribute.Name);
            }
        }

        return stringBuilder.ToString();
    }

    private string ParseProperties()
    {
        var stringBuilder = new StringBuilder();

        if (string.IsNullOrEmpty(_cachedQuery))
        {
            for (var i = 0; i < _properties.Length; i++)
            {
                var property = _properties[i];
                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var subType = property.PropertyType.GetGenericArguments()[0];
                    var subProperties = subType.GetProperties();
                    var subStringBuilder = new StringBuilder();
                    subStringBuilder.Append('(');
                    subStringBuilder.Append("SELECT ");
                    for (var j = 0; j < subProperties.Length; j++)
                    {
                        var subProperty = subProperties[j];
                        var subColumnAttribute = subProperty.GetCustomAttribute<ColumnAttribute>();
                        if (subColumnAttribute is null)
                        {
                            subStringBuilder.Append(subProperty.Name);
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(subColumnAttribute.Name))
                            {
                                subStringBuilder.Append(subProperty.Name);
                            }
                            else
                            {
                                subStringBuilder.Append(subColumnAttribute.Name);
                            }
                        }
                        if (j < subProperties.Length - 1)
                        {
                            subStringBuilder.Append(", ");
                        }
                    }

                    subStringBuilder.Append(" FROM ");
                    
                    var subTableAttribute = subType.GetCustomAttribute<TableAttribute>();
                    if (string.IsNullOrWhiteSpace(subTableAttribute?.Name))
                    {
                        subStringBuilder.Append(subType.Name);
                    }
                    else
                    {
                        subStringBuilder.Append(subTableAttribute.Name);
                    }

                    subStringBuilder.Append(')');

                    stringBuilder.Append(subStringBuilder);
                }
                else
                {
                    if (columnAttribute is null)
                    {
                        stringBuilder.Append(property.Name);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(columnAttribute.Name))
                        {
                            stringBuilder.Append(property.Name);
                        }
                        else
                        {
                            stringBuilder.Append(columnAttribute.Name);
                        }
                    }
                }

                if (i < _properties.Length - 1)
                {
                    stringBuilder.Append(", ");
                }

                _cachedQuery = stringBuilder.ToString();
            }
        }
        else
        {
            stringBuilder.Append(_cachedQuery);
        }

        return stringBuilder.ToString();
    }

    private string ParseExpression(Expression<Func<T, bool>> predicate)
    {
        var stringBuilder = new StringBuilder();

        if (predicate.Body is not BinaryExpression expression)
        {
            throw new InvalidOperationException("Invalid expression.");
        }

        var expressions = ParseBinaryExpression(expression);

        for (var i = 0; i < expressions.Count; i++)
        {
            stringBuilder.Append(expressions[i]);

            if (i < expressions.Count - 1)
            {
                stringBuilder.Append(" AND ");
            }
        }

        return stringBuilder.ToString();
    }

    private List<string> ParseBinaryExpression(BinaryExpression binaryExpression)
    {
        var expressions = new List<string>();

        if (binaryExpression is { Left: MemberExpression memberExpression, Right: ConstantExpression constantExpression})
        {
            var expressionStringBuilder = new StringBuilder();
            var property = memberExpression.Member as PropertyInfo;

            if (property is null)
            {
                throw new InvalidOperationException("Invalid expression.");
            }

            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

            if (columnAttribute is null)
            {
                expressionStringBuilder.Append(property.Name);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(columnAttribute.Name))
                {
                    expressionStringBuilder.Append(property.Name);
                }
                else
                {
                    expressionStringBuilder.Append(columnAttribute.Name);
                }
            }

            var oper = binaryExpression.NodeType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                _ => throw new InvalidOperationException("Invalid expression.")
            };

            expressionStringBuilder.Append($" {oper} ");

            if (property.PropertyType == typeof(string))
            {
                expressionStringBuilder.Append($"'{constantExpression.Value}'");
            }
            else
            {
                expressionStringBuilder.Append(constantExpression.Value!.ToString()!);
            }

            expressions.Add(expressionStringBuilder.ToString());

            return expressions;
        }

        if (binaryExpression.Left is BinaryExpression left)
        {
            expressions.AddRange(ParseBinaryExpression(left));
        }

        if (binaryExpression.Right is BinaryExpression right)
        {
            expressions.AddRange(ParseBinaryExpression(right));
        }

        return expressions;
    }
}
