namespace BookMate.API.Models;

public class BorrowRequestDto
{
    public string Email { get; set; }
    public List<string> BookIds { get; set; }
    public DateTime BorrowFrom { get; set; }
    public DateTime BorrowTo { get; set; }
}