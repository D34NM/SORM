using System.Reflection;
using System.Text;
using SORM.DataAnnotations;

namespace SORM;

/// <summary>
/// Represents a Salesforce object.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SalesforceObject<T> : SalesforceQuery<T>
    where T : class
{
    private readonly Type _type;
    private readonly PropertyInfo[] _properties;

    public SalesforceObject()
    {
        _type = typeof(T);
        _properties = _type.GetProperties();
    }

    public Task<T> FindAsync(params object[] keys)
	{
		var stringBuilder = new StringBuilder();
		stringBuilder.Append("SELECT ");

		SelectProperties(stringBuilder);

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

		var keyProperty = _properties.SingleOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);

		if (keyProperty is null)
		{
			throw new InvalidOperationException("No key attribute found.");
		}

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

		return Task.FromResult<T>(null!);
	}

	private void SelectProperties(StringBuilder stringBuilder)
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
                if (string.IsNullOrWhiteSpace(columnAttribute?.Name))
                {
                    subStringBuilder.Append(property.Name);
                }
                else
                {
                    subStringBuilder.Append(columnAttribute.Name);
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
		}
	}
}
