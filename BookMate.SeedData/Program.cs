using Amazon.DynamoDBv2;
using BookMate.SeedData;

Console.WriteLine("📚 Seeding Book Inventory...");

// Create DynamoDB client (optionally with AWS config)
var dynamoDbClient = new AmazonDynamoDBClient();

try
{
    await BookSeeder.SeedAsync(dynamoDbClient);
    Console.WriteLine("✅ Book inventory seeded successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error seeding data: {ex.Message}");
}

