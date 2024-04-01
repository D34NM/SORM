using System.Reflection;

namespace SORM.Core.Objects.Internal;

internal sealed class KeyPropertyDescriptor(PropertyInfo propertyInfo) : PropertyDescriptor(propertyInfo) { }