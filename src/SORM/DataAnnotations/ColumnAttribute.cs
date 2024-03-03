using System.Runtime.CompilerServices;

namespace SORM.DataAnnotations;

/// <summary>
/// Attribute to specify the column name for a property in Salesforce.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ColumnAttribute : Attribute
{
    private readonly string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnAttribute"/> class. When used, the property name will be used as the column name.
    /// </summary>
    public ColumnAttribute() 
    {
        _name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnAttribute"/> class. When used, the specified name will be used as the column name.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <param name="name">The name of the column in Salesforce the property is being mapped to.</param>
    public ColumnAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The column name cannot be null or empty.", nameof(name));
        }

        _name = name;
    }

    /// <summary>
    /// The name of the column the property is being mapped to.
    /// </summary>
	public string? Name { get => _name; }
}