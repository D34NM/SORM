# SORM

This project is an Object-Relational Mapping (ORM) for Salesforce. It provides a simple and intuitive way to interact with Salesforce data using C# classes. The ORM allows you to perform various operations like querying, inserting, updating, and deleting Salesforce records using C#.

## Features

- <strong>Easy to use</strong>: The ORM is designed to be simple and intuitive. You can perform operations on Salesforce data as if you were dealing with regular C# objects.
Asynchronous operations: The ORM supports asynchronous operations, making it suitable for modern, high-performance applications.
- <strong>Strongly-typed</strong>: The ORM uses C# classes to represent Salesforce objects. This means you get all the benefits of strong typing, like compile-time error checking and IntelliSense support in Visual Studio.
- <strong>Support for complex queries</strong>: The ORM supports complex queries, including nested queries and conditions.: The ORM is designed to be simple and intuitive. You can perform operations on Salesforce data as if you were dealing with regular C# objects.
- <strong>Asynchronous operations</strong>: The ORM supports asynchronous operations, making it suitable for modern, high-performance applications.
- <strong>Strongly-typed</strong>: The ORM uses C# classes to represent Salesforce objects. This means you get all the benefits of strong typing, like compile-time error checking and IntelliSense support in Visual Studio.
Support for complex queries: The ORM supports complex queries, including nested queries and conditions.

## Examples

Here are some examples of how you can use the ORM to interact with Salesforce data:

#### Querying Records

```csharp
var context = new MyContext();
var result = await context.MyCustomObjects.FindAllAsync(x => x.Name.Contains("es"));
```

This will generate and execute the following SOQL query:

```sql
SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c LIKE '%es%' LIMIT 100
```

#### Checking for Null or Empty Strings

```csharp
var context = new MyContext();
var result = await context.MyCustomObjects.FindAllAsync(x => string.IsNullOrEmpty(x.Name));
```

This will generate and execute the following SOQL query:

```sql
SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IS NULL OR Name__c = '' LIMIT 100
```

#### Limiting Results

```csharp
var context = new MyContext();
var result = await context.MyCustomObjects.FindAllAsync(x => string.IsNullOrEmpty(x.Name), 10);
```

This will generate and execute the following SOQL query:

```sql
SELECT Id,Name__c,(SELECT Id,Name__c FROM MyChildObjects__r) FROM MyCustomObject WHERE Name__c IS NULL OR Name__c = '' LIMIT 10
```

## Getting Started

To get started with the Salesforce ORM, you'll need to define your Salesforce objects as C# classes. You can then use these classes with the MyContext class to interact with your Salesforce data.

Here's an example of how to define a Salesforce object:

```csharp
[Table("MyCustomObject")]
public class MyCustomObject : SalesforceEntity
{
    [JsonPropertyName("Name__c")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("MyChildObjects__r")]
    public required Relationship<MyChildObject> MyChildObjects { get; set; }
}
```
n this example, MyCustomObject is a Salesforce object with a Name field and a relationship to MyChildObject.

## Contributing

If you'd like to contribute to the project, feel free to fork the repository and submit a pull request. You can also open an issue if you have any questions or suggestions.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
