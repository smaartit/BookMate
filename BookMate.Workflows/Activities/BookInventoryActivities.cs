using BookMate.Workflows.Contracts;
using BookMate.Workflows.Domain;
using BookMate.Workflows.Services;
using Temporalio.Activities;

namespace BookMate.Workflows.Activities;

public class BookInventoryActivities
{
    private readonly IDynamoDBService _dynamoDBService;

    public BookInventoryActivities(IDynamoDBService dynamoDBService)
    {
        _dynamoDBService = dynamoDBService;
    }

    [Activity]
    public async Task MarkBookAsUnavailableAsync(string bookId)
    {
        await _dynamoDBService.UpdateBookAvailabilityAsync(bookId, false);
    }

    [Activity]
    public async Task<List<BookDetails>> GetAvailableBooksAsync()
    {
        return await _dynamoDBService.GetAvailableBooksAsync();
    }
}