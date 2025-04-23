namespace BookMate.Workflows.Contracts;

public class BookBorrowRequest
{
    public string Email { get; set; }
    public List<string> BookIds { get; set; }
    public DateTime BorrowFrom { get; set; }
    public DateTime BorrowTo { get; set; }

    public BookBorrowRequest(string email, List<string> bookIds, DateTime borrowFrom, DateTime borrowTo)
    {
        Email = email;
        BookIds = bookIds;
        BorrowFrom = borrowFrom;
        BorrowTo = borrowTo;
    }
}