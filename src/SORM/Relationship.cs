namespace SORM;

/// <summary>
/// Represents the Salesforce relationship between two objects. This is used to deserialize the JSON response from Salesforce.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Relationship<T> where T : class
{
    public int TotalSize { get; set; }

    public IEnumerable<T> Records { get; set; }
}