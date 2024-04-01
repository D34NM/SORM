namespace SORM.Core.Objects.Internal;

internal class From(ObjectDescriptor objectDescriptor)
{
    private string _clause = string.Empty;

    public override string ToString()
    {
        return $"FROM {objectDescriptor.TableName}";
    }
}