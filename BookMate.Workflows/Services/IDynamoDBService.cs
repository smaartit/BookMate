using BookMate.Workflows.Contracts;
using BookMate.Workflows.Domain;

namespace BookMate.Workflows.Services;

public interface IDynamoDBService
{
    // Get the available books from DynamoDB
    Task<List<BookDetails>> GetAvailableBooksAsync();

    // Update the book inventory to mark the book as unavailable
    Task UpdateBookAvailabilityAsync(string bookId, bool isAvailable);

    // Add a new book to the inventory
    Task AddBookAsync(BookDetails book);
}