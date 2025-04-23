using BookMate.API.Models;
using BookMate.Workflows.Contracts;
using BookMate.Workflows.Workflows;
using Microsoft.AspNetCore.Mvc;
using Temporalio.Client;
namespace BookMate.API;

[ApiController]
[Route("api/borrow")]
public class BorrowController : ControllerBase
{
    private readonly TemporalClient _temporalClient;

    public BorrowController(TemporalClient temporalClient)
    {
        _temporalClient = temporalClient;
    }

    // Borrow books endpoint
    [HttpPost]
    public async Task<IActionResult> BorrowBooks([FromBody] BorrowRequestDto request)
    {
        var workflowRequest = new BookBorrowRequest(
            request.Email,
            request.BookIds,
            request.BorrowFrom,
            request.BorrowTo
        );

        // Start the Temporal workflow for borrowing books
        await _temporalClient.StartWorkflowAsync<BookBorrowWorkflow>(
            (wf) => wf.RunAsync(workflowRequest),
            new WorkflowOptions()
            {
                Id = $"borrow-{Guid.NewGuid()}",
                TaskQueue = "bookmate-queue",
            });

        return Accepted("Borrowing in progress. You'll receive a confirmation email shortly.");
    }

    // Get available books endpoint
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableBooks([FromServices] IBookInventoryActivities inventory)
    {
        var availableBooks = await inventory.GetAvailableBooksAsync();
        return Ok(availableBooks);
    }
}
