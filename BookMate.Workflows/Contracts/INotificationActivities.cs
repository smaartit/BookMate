namespace BookMate.Workflows.Contracts;

public interface INotificationActivities
{
    Task SendBorrowConfirmationAsync(string email, List<string> bookIds, DateTime borrowTo);
}