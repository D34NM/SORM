using SORM.DataAnnotations;

namespace SORM.Core.Objects;

/// <summary>
/// Represents a Salesforce entity with a specified key type. This is used to deserialize the JSON response from Salesforce.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <remarks>
/// The key type should be a string or an integer.
/// </remarks>
/// <example>
/// <code>
/// public class MyObject : SalesforceEntity
/// {
///    [JsonPropertyName("Name__c")]
///    public string Name { get; set; } = string.Empty;
///    [JsonPropertyName("MyChildObjects__r")]
///    public required Relationship<MyChildObject> MyChildObjects { get; set; }
///    [JsonPropertyName("MyChildObject__r")]
///    public required MyChildObject MyChildObject { get; set; }
/// }
/// </code>
/// </example>
public abstract class SalesforceObject<TKey>
{
    [Key]
    public required TKey Id { get; set; }

    [ResponseOnly]
    public required Attributes Attributes { get; set; }
}

/// <summary>
/// Represents a Salesforce entity with a string key. This is used to deserialize the JSON response from Salesforce.
/// </summary>
public abstract class SalesforceObject : SalesforceObject<string> { }

public sealed class Attributes
{
    public required string Type { get; set; }
    public required string Url { get; set; }
}