namespace SORM.DataAnnotations;

/// <summary>
/// Attribute to specify the key name for a mapped Type in Salesforce.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class KeyAttribute : Attribute
{
    private readonly string _name;

    public KeyAttribute()
    {
        _name = string.Empty;
    }

    public KeyAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The key name cannot be null or empty.", nameof(name));
        }

        _name = name;
    }

    public string Name { get => _name; }
}