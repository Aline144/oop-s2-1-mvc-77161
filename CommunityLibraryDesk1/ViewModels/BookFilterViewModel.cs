using CommunityLibraryDesk1.Models;

namespace CommunityLibraryDesk1.ViewModels;

public class BookFilterViewModel
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public string? Availability { get; set; }
    public List<string> Categories { get; set; } = new();
    public List<Book> Books { get; set; } = new();
}