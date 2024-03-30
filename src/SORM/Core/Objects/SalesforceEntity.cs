using SORM.DataAnnotations;

namespace SORM.Core.Objects;

/// <summary>
/// Represents a Salesforce entity. This is used to deserialize the JSON response from Salesforce.
/// </summary>
public abstract class SalesforceEntity
{
    [Key]
    public required string Id { get; set; }

    [ResponseOnly]
    public required Attributes Attributes { get; set; }
}

public sealed class Attributes
{
    public required string Type { get; set; }
    public required string Url { get; set; }
}