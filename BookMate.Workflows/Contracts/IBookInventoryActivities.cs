using BookMate.Workflows.Domain;

namespace BookMate.Workflows.Contracts;

public interface IBookInventoryActivities
{
    Task MarkBookAsUnavailableAsync(string bookId);
    Task<List<BookDetails>> GetAvailableBooksAsync();
}