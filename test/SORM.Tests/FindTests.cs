using System.Text.Json.Serialization;
using SORM.DataAnnotations;

namespace SORM.Tests;

public class FindTests
{
    [Fact(DisplayName = "FindAsync returns SELECT Id,Name__c FROM MyObject WHERE Id = 'Id'")]
    public void Test1()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyObjects.FindAsync("Id");

		// Assert
		Assert.Equal("SELECT Id,Name__c FROM MyObject WHERE Id = 'Id'", result);
    }

    [Fact(DisplayName = "FindAsync with inner SELECT query returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Id = 'Id'")]
    public void Test2()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects.FindAsync("Id");

        // Assert
        Assert.Equal("SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Id = 'Id'", result);
    }

    [Fact(DisplayName = "FindAllAsync returns SELECT Id,Name__c FROM MyObject LIMIT 100")]
    public void Test3()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyObjects.FindAllAsync();

        // Assert
        Assert.Equal("SELECT Id,Name__c FROM MyObject LIMIT 100", result);
    }

    [Fact(DisplayName = "FindAllAsync with inner SELECT query returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject LIMIT 100")]
    public void Test4()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects.FindAllAsync();

        // Assert
        Assert.Equal("SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject LIMIT 100", result);
    }

    [Fact(DisplayName = "FindAllAsync with LIMIT clause returns SELECT Id,Name__c FROM MyObject LIMIT 10")]
    public void Test5()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyObjects.FindAllAsync(10);

        // Assert
        Assert.Equal("SELECT Id,Name__c FROM MyObject LIMIT 10", result);
    }

    [Fact(DisplayName = "FindAllAsync with WHERE expression and LIMIT clause returns SELECT Id,Name__c FROM MyObject WHERE Id = 'Id' LIMIT 10")]
    public void Test6()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyObjects.FindAllAsync(x => x.Id == "Id", 10);

        // Assert
        Assert.Equal("SELECT Id,Name__c FROM MyObject WHERE Id = 'Id' LIMIT 10", result);
    }

    [Fact(DisplayName = "FindAllAsync with complex WHERE expression clause returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Id = 'Id' AND Name__c = 'test' LIMIT 100")]
    public void Test7()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects
                .FindAllAsync(x => x.Id == "Id" && x.Name == "test");

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Id = 'Id' AND Name__c = 'test' LIMIT 100",
            result);
    }

    [Fact(DisplayName = "FindAllAsync with complex WHERE expression and Order By and LIMIT clause returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Id = 'Id' AND Name__c = 'test' ORDER BY Name__c ASC NULLS FIRST LIMIT 10")]
    public void Test8()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects
                .FindAllAsync(
                    x => x.Id == "Id" && x.Name == "test", 
                    by => by.Column(
                        x => x.Name, 
                        Direction.Ascending, 
                        Nulls.First, 
                        Limit.By(10)));

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Id = 'Id' AND Name__c = 'test' ORDER BY Name__c ASC NULLS FIRST LIMIT 10 OFFSET 0", 
            result);
    }

    [Fact(DisplayName = "FindAllAsync with IN WHERE expression returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IN ('Name 1','Name 2') LIMIT 100")]
    public void Test9()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects
                .FindAllAsync(
                    x => new[] { "Name 1", "Name 2" }.Contains(x.Name));

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IN ('Name 1','Name 2') LIMIT 100", 
            result);
    }

    [Fact(DisplayName = "FindAllAsync with IN WHERE expression returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IN ('Name 1','Name 2') AND Id = 'Id' LIMIT 100")]
    public void Test10()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects
                .FindAllAsync(
                    x => new[] { "Name 1", "Name 2" }.Contains(x.Name) && x.Id == "Id");

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IN ('Name 1','Name 2') AND Id = 'Id' LIMIT 100", 
            result);
    }
}

#region Test Helpers

[Table("MyObject")]
public class MyObject
{
    [Key]
    public required string Id { get; set; }

    [JsonPropertyName("Name__c")]
    public string Name { get; set; } = string.Empty;
}

[Table("MyCustomObject")]
public class MyCustomObject
{
    [Key]
    public required string Id { get; set; }

    [JsonPropertyName("Name__c")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("MyChildObjects__r")]
    public required Relationship<MyChildObject> MyChildObjects { get; set; }
}

[Table("MyChildObject")]
public class MyChildObject
{
    [Key]
    public required string Id { get; set; }

    [JsonPropertyName("Name__c")]
    public string Name { get; set; } = string.Empty;
}

public class MyContext : SalesforceContext
{
    public SalesforceObject<MyObject> MyObjects { get; }
    public SalesforceObject<MyCustomObject> MyCustomObjects { get; }

    public MyContext()
    {
        MyObjects = new SalesforceObject<MyObject>();
        MyCustomObjects = new SalesforceObject<MyCustomObject>();
    }
}

#endregion