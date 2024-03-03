namespace SORM.DataAnnotations;

/// <summary>
/// Attribute to specify the table name for a class in Salesforce.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TableAttribute : Attribute
{
    private readonly string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableAttribute"/> class. When used, the class name will be used as the table name.
    /// </summary>
    public TableAttribute() 
    {
        _name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableAttribute"/> class. When used, the specified name will be used as the table name.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <param name="name">The name of the table in Salesforce the class is being mapped to.</param>
    public TableAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The table name cannot be null or empty.", nameof(name));
        }

        _name = name;
    }

    /// <summary>
    /// The name of the table the class is being mapped to.
    /// </summary>
    public string Name { get => _name; }
}