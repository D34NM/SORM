using SORM.Core.Objects;

namespace SORM.Tests;

public class FindTests
{
    [Fact(DisplayName = "FindAllAsync returns SELECT Id,Name__c FROM MyObject LIMIT 100")]
    public void Test1()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyObjects.FindAllAsync();

        // Assert
        Assert.Equal("SELECT Id,Name__c FROM MyObject LIMIT 100", result);
    }

    [Fact(DisplayName = "FindAllAsync with inner SELECT query returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject LIMIT 100")]
    public void Test2()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects.FindAllAsync();

        // Assert
        Assert.Equal("SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject LIMIT 100", result);
    }

    [Fact(DisplayName = "FindAllAsync with LIMIT clause returns SELECT Id,Name__c FROM MyObject LIMIT 10")]
    public void Test3()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyObjects.FindAllAsync(10);

        // Assert
        Assert.Equal("SELECT Id,Name__c FROM MyObject LIMIT 10", result);
    }

    [Fact(DisplayName = "FindAllAsync with WHERE expression and LIMIT clause returns SELECT Id,Name__c FROM MyObject WHERE Id = 'Id' LIMIT 10")]
    public void Test4()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyObjects.FindAllAsync(x => x.Id == "Id", 10);

        // Assert
        Assert.Equal("SELECT Id,Name__c FROM MyObject WHERE Id = 'Id' LIMIT 10", result);
    }

    [Fact(DisplayName = "FindAllAsync with complex WHERE expression clause returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Id = 'Id' AND Name__c = 'test' LIMIT 100")]
    public void Test5()
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
    public void Test6()
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
                        NullsOrdering.First, 
                        Limit.By(10)));

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Id = 'Id' AND Name__c = 'test' ORDER BY Name__c ASC NULLS FIRST LIMIT 10 OFFSET 0", 
            result);
    }

    [Fact(DisplayName = "FindAllAsync with IN WHERE expression returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IN ('Name 1','Name 2') LIMIT 100")]
    public void Test7()
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

    [Fact(DisplayName = "FindAllAsync with Enumerable.Contains WHERE expression returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IN ('Name 1','Name 2') AND Id = 'Id' LIMIT 100")]
    public void Test8()
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

    [Fact(DisplayName = "FindAllAsync with string.StartsWith WHERE expression returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c LIKE 'Tes%' LIMIT 100")]
    public void Test9()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects
                .FindAllAsync(
                    x => x.Name.StartsWith("Tes"));

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c LIKE 'Tes%' LIMIT 100", 
            result);
    }

    [Fact(DisplayName = "FindAllAsync with string.EndsWith WHERE expression returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c LIKE '%t' LIMIT 100")]
    public void Test10()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects
                .FindAllAsync(
                    x => x.Name.EndsWith("t"));

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c LIKE '%t' LIMIT 100", 
            result);
    }

    [Fact(DisplayName = "FindAllAsync with string.Contains WHERE expression and LIMIT clause returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c LIKE '%es%' LIMIT 100")]
    public void Test11()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects
                .FindAllAsync(
                    x => x.Name.Contains("es"));

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c LIKE '%es%' LIMIT 100", 
            result);
    }

    [Fact(DisplayName = "FindAllAsync with string.IsNullOrEmpty WHERE expression returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IS NULL OR Name__c = '' LIMIT 100")]
    public void Test12()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects
                .FindAllAsync(
                    x => string.IsNullOrEmpty(x.Name));

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IS NULL OR Name__c = '' LIMIT 100", 
            result);
    }

    [Fact(DisplayName = "FindAllAsync with string.IsNullOrWhiteSpace WHERE expression returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IS NULL OR Name__c = '' LIMIT 100")]
    public void Test13()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects
                .FindAllAsync(
                    x => string.IsNullOrWhiteSpace(x.Name));

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IS NULL OR Name__c = '' LIMIT 100", 
            result);
    }

    [Fact(DisplayName = "FindAllAsync with string.IsNullOrEmpty WHERE expression and LIMIT clause returns SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IS NULL OR Name__c = '' LIMIT 10")]
    public void Test14()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyCustomObjects
                .FindAllAsync(
                    x => string.IsNullOrEmpty(x.Name), 
                    10);

        // Assert
        Assert.Equal(
            "SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IS NULL OR Name__c = '' LIMIT 10", 
            result);
    }

    [Fact(DisplayName = "FindAllAsync with SalesforceObject WHERE expression returns SELECT Id,Name__c, MyChildObject__r.Name__c FROM MyObjectWithChild WHERE Id = 'Id' LIMIT 100")]
    public void Test15()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyObjectWithChild.FindAllAsync(x => x.Id == "Id");

        // Assert
        Assert.Equal("SELECT Id,Name__c,MyChildObject__r.Id,MyChildObject__r.Name__c FROM MyObjectWithChild WHERE Id = 'Id' LIMIT 100", result);
    }
}