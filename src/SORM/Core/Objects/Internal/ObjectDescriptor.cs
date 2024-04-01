using System.Reflection;
using SORM.DataAnnotations;

namespace SORM.Core.Objects.Internal;

internal class ObjectDescriptor : Descriptor
{
    private readonly Type _type;
    private readonly KeyPropertyDescriptor _key;
    private readonly List<PropertyDescriptor> _properties;

    public string Name { get; }

    public string TableName { get; }

    public KeyPropertyDescriptor Key => _key;

    public ObjectDescriptor(Type type)
    {
        _type = type;
        _properties = [];

        Name = _type.Name;
        TableName = _type.GetCustomAttribute<TableAttribute>()?.Name ?? _type.Name;

        var properties = type
            .GetProperties()
            .Where(p => p.GetCustomAttribute<ResponseOnlyAttribute>() == null);

        foreach (var property in properties.Where(p => p.DeclaringType == typeof(SalesforceEntity)))
        {
            if (property.GetCustomAttribute<KeyAttribute>() != null ||
                property.Name == nameof(SalesforceEntity.Id))
            {
                _key = new KeyPropertyDescriptor(property);
                _properties.Add(_key);
                continue;
            }

            _properties.Add(new PropertyDescriptor(property));
        }

        foreach (var property in properties.Where(p => p.DeclaringType == type))
        {
            if (property.PropertyType.IsClass &&
                property.PropertyType != typeof(string) &&
                property.PropertyType.IsGenericType &&
                property.PropertyType.GetGenericTypeDefinition() == typeof(Relationship<>))
            {
                _properties.Add(new RelationshipDescriptor(property));
                continue;
            }

            _properties.Add(new PropertyDescriptor(property));
        }

        if (_key == null)
        {
            throw new InvalidOperationException($"Entity {_type.Name} does not have a key property.");
        }
    }

    public override Descriptor[] GetProperties()
    {
        return [.. _properties];
    }

    public override string ToString()
    {
        return TableName;
    }
}