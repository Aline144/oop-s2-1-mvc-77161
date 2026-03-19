using System.ComponentModel.DataAnnotations;

namespace CommunityLibraryDesk1.Models;

public class Member
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}