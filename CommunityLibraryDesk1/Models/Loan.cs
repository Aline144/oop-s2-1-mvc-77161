using System.ComponentModel.DataAnnotations;

namespace CommunityLibraryDesk1.Models;

public class Loan
{
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Required]
    public DateTime LoanDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    public DateTime? ReturnedDate { get; set; }

    public Book? Book { get; set; }
    public Member? Member { get; set; }

    public bool IsOverdue => ReturnedDate == null && DueDate.Date < DateTime.Today;
}