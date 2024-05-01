using System.Text.Json.Serialization;
using SORM.Core.Objects;
using SORM.DataAnnotations;

namespace SORM.Tests;

[Table("MyObject")]
public class MyObject : SalesforceObject
{
    [JsonPropertyName("Name__c")]
    public string Name { get; set; } = string.Empty;
}

[Table("MyCustomObject")]
public class MyCustomObject : SalesforceObject 
{
    [JsonPropertyName("Name__c")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("MyChildObjects__r")]
    public required Relationship<MyChildObject> MyChildObjects { get; set; }
}

[Table("MyChildObject")]
public class MyChildObject : SalesforceObject
{
    [JsonPropertyName("Name__c")]
    public string Name { get; set; } = string.Empty;
}

[Table("MyObjectWithChild")]
public class MyObjectWithChild : SalesforceObject
{
    [JsonPropertyName("Name__c")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("MyChildObject__r")]
    public required MyChildObject MyChildObject { get; set; }
}

public class MyContext : SalesforceContext
{
    public Queryable<MyObject> MyObjects { get; }
    public Queryable<MyCustomObject> MyCustomObjects { get; }
    public Queryable<MyObjectWithChild> MyObjectWithChild { get; }

    public MyContext()
    {
        MyObjects = new Queryable<MyObject>();
        MyCustomObjects = new Queryable<MyCustomObject>();
        MyObjectWithChild = new Queryable<MyObjectWithChild>();
    }
}