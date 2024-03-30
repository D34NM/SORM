using System.Reflection;

namespace SORM.Core.Objects.Internal;

internal class Column
{
    private readonly PropertyInfo _property;
    private readonly string _name;

    public Column(PropertyInfo property)
    {
        _property = property;
        _name = _property.GetDecoratedOrPropertyName();
    }

    public override string ToString()
    {
        return _name;
    }
}
