namespace BookMate.Workflows.Contracts;

public class BorrowConfirmation
{
    public string Email { get; set; }
    public List<string> BookIds { get; set; }
    public DateTime BorrowTo { get; set; }
}