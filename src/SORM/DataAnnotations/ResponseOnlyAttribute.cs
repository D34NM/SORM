namespace SORM.DataAnnotations;

/// <summary>
/// Attribute to specify that a property should only be included in the response from Salesforce.
/// </summary>
/// 
/// <example>
/// <code>
/// [ResponseOnly]
/// public string Password { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ResponseOnlyAttribute : Attribute;