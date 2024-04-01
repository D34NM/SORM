namespace SORM.Core.Objects.Internal;

internal abstract class Descriptor
{
	public virtual bool IsRelationship { get; } = false;
	
	public virtual Descriptor[] GetProperties()
    {
        return [];
    }
}