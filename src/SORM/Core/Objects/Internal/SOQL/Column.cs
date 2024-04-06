namespace SORM.Core.Objects.Internal.SOQL;

internal abstract class Column()
{
	public override string ToString()
    {
        throw new InvalidOperationException("Column name cannot be null");
    }
}
