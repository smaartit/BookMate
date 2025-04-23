using BookMate.Workflows.Activities;
using BookMate.Workflows.Contracts;
using Temporalio.Common;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace BookMate.Workflows.Workflows;

[Workflow]
public class BookBorrowWorkflow
{
    [WorkflowRun]
    public async Task<BorrowConfirmation> RunAsync(BookBorrowRequest request)
    {
        var activityOptions = new ActivityOptions
        {
            StartToCloseTimeout = TimeSpan.FromSeconds(15),
            RetryPolicy = new RetryPolicy
            {
                MaximumAttempts = 1
            }
        };
        try
        {
            
            foreach (var bookId in request.BookIds)
            {
                await Workflow.ExecuteActivityAsync(
                    (BookInventoryActivities a) => a.MarkBookAsUnavailableAsync(bookId),
                    activityOptions);
            }
        
            // Send confirmation email
            
            await Workflow.ExecuteActivityAsync(
                (NotificationActivities n) => n.SendBorrowConfirmationAsync(request.Email, request.BookIds, request.BorrowTo),
                activityOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Workflow failed due to: {ex.Message}");
            throw new Exception("Workflow failed while borrowing books.", ex);
        }

        return new BorrowConfirmation
        {
            Email = request.Email,
            BookIds = request.BookIds,
            BorrowTo = request.BorrowTo
        };
    }
}