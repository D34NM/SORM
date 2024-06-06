namespace SORM.Tests;

public class FindByIdTests
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

    [Fact(DisplayName = "FindAsync with WHERE expression returns SELECT Id,Name__c FROM MyObject WHERE Id = '1234'")]
    public void Test3()
    {
        // Arrange
        var context = new MyContext();

        // Act
        var result = context.MyObjects.FindAsync("1234");

        // Assert
        Assert.Equal("SELECT Id,Name__c FROM MyObject WHERE Id = '1234'", result);
    }
}
