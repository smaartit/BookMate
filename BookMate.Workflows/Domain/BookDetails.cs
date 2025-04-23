namespace BookMate.Workflows.Domain;

public class BookDetails
{
    public string BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public bool IsAvailable { get; set; }
}