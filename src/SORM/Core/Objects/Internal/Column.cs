namespace SORM.Core.Objects.Internal;

internal abstract class Column()
{
	public override string ToString()
    {
        throw new InvalidOperationException("Column name cannot be null");
    }
}
