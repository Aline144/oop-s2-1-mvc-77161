using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CommunityLibraryDesk1.ViewModels;

public class LoanCreateViewModel
{
    [Required]
    [Display(Name = "Book")]
    public int BookId { get; set; }

    [Required]
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime LoanDate { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(14);

    public List<SelectListItem> AvailableBooks { get; set; } = new();
    public List<SelectListItem> Members { get; set; } = new();
}