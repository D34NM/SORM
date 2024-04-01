namespace SORM.Core.Objects.Internal;

internal abstract class Descriptor
{
    public virtual bool IsRelationship { get; }

    public virtual bool IsResponseOnly { get; }

    public virtual Descriptor[] GetProperties()
    {
        return [];
    }
}