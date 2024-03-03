using SORM.DataAnnotations;

namespace SORM.Tests;

public class FindTests
{
    [Fact(DisplayName = "FindAsync returns null")]
    public async Task Test1()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = await context.MyObjects.FindAsync("Id");

		// Assert
		Assert.Null(result);
    }

    [Fact(DisplayName = "FindAsync with inner SELECT query returns null")]
    public async Task Test2()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = await context.MyCustomObjects.FindAsync("Id");

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "FindAllAsync returns null")]
    public async Task Test3()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = await context.MyObjects.FindAllAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "FindAllAsync with inner SELECT query returns null")]
    public async Task Test4()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = await context.MyCustomObjects.FindAllAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "FindAllAsync with LIMIT clause returns null")]
    public async Task Test5()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = await context.MyObjects.FindAllAsync(10);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "FindAllAsync with WHERE expression and LIMIT clause returns null")]
    public async Task Test6()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = await context.MyObjects.FindAllAsync(x => x.Id == "Id", 10);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "FindAllAsync with complex WHERE expression clause returns null")]
    public async Task Test7()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = await context.MyCustomObjects.FindAllAsync(x => x.Id == "Id" && x.Name == "test");

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "FindAllAsync with complex WHERE expression and Order By and LIMIT clause returns null")]
    public async Task Test8()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = await context.MyCustomObjects.FindAllAsync(x => x.Id == "Id" && x.Name == "test", x => x.Name, false, 10);

        // Assert
        Assert.Null(result);
    }
}

#region Test Helpers

[Table("MyObject")]
public class MyObject
{
    [Column, Key]
    public required string Id { get; set; }

    [Column("Name__c")]
    public string Name { get; set; } = string.Empty;
}

[Table("MyCustomObject")]
public class MyCustomObject
{
    [Column, Key]
    public required string Id { get; set; }

    [Column("Name__c")]
    public string Name { get; set; } = string.Empty;

    [Column("MyChildObjects__c")]
    public List<MyChildObject> MyChildObjects { get; set; } = [];
}

[Table("MyChildObject")]
public class MyChildObject
{
    [Column, Key]
    public required string Id { get; set; }

    [Column("Name__c")]
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