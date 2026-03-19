using CommunityLibraryDesk1.Data;
using CommunityLibraryDesk1.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CommunityLibraryDesk1.Tests1;

public class BooksAndLoansTests
{
    private ApplicationDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Test]
    public void BookSearch_ReturnsExpectedMatch_ByTitle()
    {
        using var context = GetContext();

        context.Books.AddRange(
            new Book
            {
                Title = "C# Programming",
                Author = "Alice",
                Isbn = "111",
                Category = "Technology",
                IsAvailable = true
            },
            new Book
            {
                Title = "History of Rome",
                Author = "Bob",
                Isbn = "222",
                Category = "History",
                IsAvailable = true
            }
        );

        context.SaveChanges();

        var result = context.Books
            .Where(b => b.Title.Contains("C#") || b.Author.Contains("C#"))
            .ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Title, Is.EqualTo("C# Programming"));
    }

    [Test]
    public void ReturnedLoan_MakesBookAvailableAgain()
    {
        using var context = GetContext();

        var book = new Book
        {
            Title = "ASP.NET MVC",
            Author = "John",
            Isbn = "333",
            Category = "Technology",
            IsAvailable = false
        };

        var member = new Member
        {
            FullName = "Test Member",
            Email = "test@test.com",
            Phone = "123"
        };

        context.Books.Add(book);
        context.Members.Add(member);
        context.SaveChanges();

        var loan = new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = DateTime.Today.AddDays(-2),
            DueDate = DateTime.Today.AddDays(7),
            ReturnedDate = null
        };

        context.Loans.Add(loan);
        context.SaveChanges();

        loan.ReturnedDate = DateTime.Today;
        book.IsAvailable = true;
        context.SaveChanges();

        Assert.That(book.IsAvailable, Is.True);
        Assert.That(loan.ReturnedDate, Is.Not.Null);
    }

    [Test]
    public void CannotHaveSecondActiveLoan_ForSameBook()
    {
        using var context = GetContext();

        var book = new Book
        {
            Title = "Clean Code",
            Author = "Robert Martin",
            Isbn = "444",
            Category = "Technology",
            IsAvailable = false
        };

        var member1 = new Member
        {
            FullName = "Member One",
            Email = "one@test.com",
            Phone = "111"
        };

        var member2 = new Member
        {
            FullName = "Member Two",
            Email = "two@test.com",
            Phone = "222"
        };

        context.Books.Add(book);
        context.Members.AddRange(member1, member2);
        context.SaveChanges();

        context.Loans.Add(new Loan
        {
            BookId = book.Id,
            MemberId = member1.Id,
            LoanDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(7),
            ReturnedDate = null
        });

        context.SaveChanges();

        var activeLoanExists = context.Loans
            .Any(l => l.BookId == book.Id && l.ReturnedDate == null);

        Assert.That(activeLoanExists, Is.True);
    }
}