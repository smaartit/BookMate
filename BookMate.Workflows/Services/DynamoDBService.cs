
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BookMate.Workflows.Contracts;
using BookMate.Workflows.Domain;

namespace BookMate.Workflows.Services;

public class DynamoDBService : IDynamoDBService
{
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private const string TableName = "BookInventory"; // DynamoDB table name

        public DynamoDBService(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        // Get all available books from DynamoDB
        public async Task<List<BookDetails>> GetAvailableBooksAsync()
        {
            var scanRequest = new ScanRequest
            {
                TableName = TableName,
                FilterExpression = "isAvailable = :isAvailable",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":isAvailable", new AttributeValue { BOOL = true } }
                }
            };

            var result = await _dynamoDbClient.ScanAsync(scanRequest);
            var books = result.Items.Select(item => new BookDetails()
            {
                BookId = item["BookId"].S,
                Title = item["Title"].S,
                Author = item["Author"].S,
                IsAvailable = item["isAvailable"].BOOL
            }).ToList();

            return books;
        }

        // Update book availability in DynamoDB
        public async Task UpdateBookAvailabilityAsync(string bookId, bool isAvailable)
        {
            var updateItemRequest = new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "BookId", new AttributeValue { S = bookId } }
                },
                UpdateExpression = "SET isAvailable = :isAvailable",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":isAvailable", new AttributeValue { BOOL = isAvailable } }
                }
            };

            await _dynamoDbClient.UpdateItemAsync(updateItemRequest);
        }

        // Add a new book to DynamoDB
        public async Task AddBookAsync(BookDetails book)
        {
            var putItemRequest = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "BookId", new AttributeValue { S = book.BookId } },
                    { "Title", new AttributeValue { S = book.Title } },
                    { "Author", new AttributeValue { S = book.Author } },
                    { "isAvailable", new AttributeValue { BOOL = book.IsAvailable } }
                }
            };

            await _dynamoDbClient.PutItemAsync(putItemRequest);
        }
}