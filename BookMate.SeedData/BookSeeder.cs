using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using BookMate.Workflows.Domain;

namespace BookMate.SeedData;
public static class BookSeeder
{
    public static async Task SeedAsync(IAmazonDynamoDB dynamoDb)
    {
        const string tableName = "BookInventory";

        // Optionally, check if the table exists and create it if it doesn't
        var tables = await dynamoDb.ListTablesAsync();
        if (!tables.TableNames.Contains(tableName))
        {
            await dynamoDb.CreateTableAsync(new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new("BookId", ScalarAttributeType.S)
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new("BookId", KeyType.HASH)
                },
                ProvisionedThroughput = new ProvisionedThroughput(5, 5)
            });

            Console.WriteLine("📘 Created BookInventory table. Waiting for it to become active...");
            await WaitForTableActive(dynamoDb, tableName);
        }

        // Read books from the JSON file
        var booksJson = File.ReadAllText("books.json");
        var books = JsonConvert.DeserializeObject<List<BookDetails>>(booksJson);

        foreach (var book in books)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                ["BookId"] = new AttributeValue(book.BookId),
                ["Title"] = new AttributeValue(book.Title),
                ["Author"] = new AttributeValue(book.Author),
                ["IsAvailable"] = new AttributeValue { BOOL = book.IsAvailable }
            };

            await dynamoDb.PutItemAsync(tableName, item);
            Console.WriteLine($"✅ Seeded book: {book.Title}");
        }
    }

    private static async Task WaitForTableActive(IAmazonDynamoDB dynamoDb, string tableName)
    {
        while (true)
        {
            var response = await dynamoDb.DescribeTableAsync(tableName);
            if (response.Table.TableStatus == TableStatus.ACTIVE)
                break;

            await Task.Delay(1000);
        }
    }
}
