using CommunityLibraryDesk1.Models;
using NUnit.Framework;

namespace CommunityLibraryDesk1.Tests1;

public class LoanTests
{
    [Test]
    public void IsOverdue_ReturnsTrue_WhenDueDateIsBeforeToday_AndNotReturned()
    {
        var loan = new Loan
        {
            DueDate = DateTime.Today.AddDays(-1),
            ReturnedDate = null
        };

        Assert.That(loan.IsOverdue, Is.True);
    }

    [Test]
    public void IsOverdue_ReturnsFalse_WhenReturnedDateIsSet()
    {
        var loan = new Loan
        {
            DueDate = DateTime.Today.AddDays(-5),
            ReturnedDate = DateTime.Today
        };

        Assert.That(loan.IsOverdue, Is.False);
    }
}