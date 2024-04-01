namespace SORM.Core.Objects.Internal;

internal class From(ObjectDescriptor objectDescriptor)
{
    public override string ToString()
    {
        return $"FROM {objectDescriptor.TableName}";
    }
}