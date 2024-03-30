namespace SORM.Core.Objects;

/// <summary>
/// Represents an OFFSET clause in a SOQL query.
/// <seealso cref="https://developer.salesforce.com/docs/atlas.en-us.soql_sosl.meta/soql_sosl/sforce_api_calls_sosl_offset.htm"/>
/// </summary>
public sealed class Offset
{
    private readonly uint _offset = 0;
    private string _clause = string.Empty;

    private Offset(uint offset)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan<uint>(offset, 2000);

        _offset = offset;
    }

    public static Offset By(uint offset)
    {
        return new(offset);
    }

    /// <summary>
    /// The maximum offset is 2,000 rows. Requesting an offset greater than 2,000 will result in a System.SearchException: SOSL offset should be between 0 to 2000 error.
    /// </summary>
    public static Offset Max => new(2000);

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(_clause))
        {
            return _clause;
        }

        _clause = $"OFFSET {_offset}";

        return _clause;
    }
}
