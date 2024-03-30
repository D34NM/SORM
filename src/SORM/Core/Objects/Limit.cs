namespace SORM.Core.Objects;

/// <summary>
/// Represents a LIMIT clause in a SOQL query.
/// <seealso cref="https://developer.salesforce.com/docs/atlas.en-us.soql_sosl.meta/soql_sosl/sforce_api_calls_sosl_limit.htm"/>
/// </summary>
public sealed class Limit
{
    private readonly uint _limit = 100;
    private string _clause = string.Empty;

    private Limit(uint limit)
    {
		ArgumentOutOfRangeException.ThrowIfGreaterThan<uint>(limit, 2000);

		_limit = limit;
    }

	public static Limit By(uint limit) => new(limit);

    /// <summary>
    /// The default 2,000 results is the largest number of rows that can be returned for API version 28.0 and later.
    /// </summary>
	public static Limit Max => new(2000);

	public override string ToString()
    {
        if (!string.IsNullOrEmpty(_clause))
        {
            return _clause;
        }

        _clause = $"LIMIT {_limit}";

        return _clause;
    }
}
