using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using BookMate.Workflows.Contracts;
using Temporalio.Activities;

namespace BookMate.Workflows.Activities;

public class NotificationActivities
{
    private readonly IAmazonSimpleEmailService _sesClient;

    public NotificationActivities(IAmazonSimpleEmailService sesClient)
    {
        _sesClient = sesClient;
    }
    
    // Send email confirmation
    [Activity]
    public async Task SendBorrowConfirmationAsync(string email, List<string> bookIds, DateTime borrowTo)
    {
        try
        {
            var sendRequest = new SendEmailRequest
            {
                Source = "asma.mahmuda@outlook.com",
                Destination = new Destination
                {
                    ToAddresses = [email]
                },
                Message = new Message
                {
                    Subject = new Content("Book Borrow Confirmation"),
                    Body = new Body
                    {
                        Html = new Content($"<p>You have borrowed the following books: <strong>{string.Join(", ", bookIds)}</strong>. Please return them by <strong>{borrowTo.ToShortDateString()}</strong>.</p>")
                    }
                }
            };

            // Send the email using AWS SES
            var response = await _sesClient.SendEmailAsync(sendRequest);

            Console.WriteLine($"Email sent successfully. Message ID: {response.MessageId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email. Error: {ex.Message}");
        }
    }
}