using Bogus;
using CommunityLibraryDesk1.Data;
using CommunityLibraryDesk1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunityLibraryDesk1.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        await context.Database.MigrateAsync();

        await SeedRolesAndAdminAsync(roleManager, userManager);

        if (!context.Books.Any())
        {
            var categories = new[] { "Fiction", "Science", "History", "Technology", "Children" };

            var bookFaker = new Faker<Book>()
                .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Isbn, f => f.Random.ReplaceNumbers("978##########"))
                .RuleFor(b => b.Category, f => f.PickRandom(categories))
                .RuleFor(b => b.IsAvailable, true);

            var books = bookFaker.Generate(20);
            context.Books.AddRange(books);
            await context.SaveChangesAsync();
        }

        if (!context.Members.Any())
        {
            var memberFaker = new Faker<Member>()
                .RuleFor(m => m.FullName, f => f.Name.FullName())
                .RuleFor(m => m.Email, f => f.Internet.Email())
                .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber());

            var members = memberFaker.Generate(10);
            context.Members.AddRange(members);
            await context.SaveChangesAsync();
        }

        if (!context.Loans.Any())
        {
            var books = await context.Books.ToListAsync();
            var members = await context.Members.ToListAsync();

            var random = new Random();
            var usedBookIds = new HashSet<int>();
            var loans = new List<Loan>();

            for (int i = 0; i < 15; i++)
            {
                var availableBooks = books.Where(b => !usedBookIds.Contains(b.Id)).ToList();
                if (!availableBooks.Any()) break;

                var book = availableBooks[random.Next(availableBooks.Count)];
                var member = members[random.Next(members.Count)];

                var loanDate = DateTime.Today.AddDays(-random.Next(1, 30));
                var dueDate = loanDate.AddDays(14);

                DateTime? returnedDate = null;

                if (i < 5)
                {
                    returnedDate = dueDate.AddDays(-random.Next(1, 5));
                    book.IsAvailable = true;
                }
                else
                {
                    usedBookIds.Add(book.Id);
                    book.IsAvailable = false;
                }

                loans.Add(new Loan
                {
                    BookId = book.Id,
                    MemberId = member.Id,
                    LoanDate = loanDate,
                    DueDate = dueDate,
                    ReturnedDate = returnedDate
                });
            }

            context.Loans.AddRange(loans);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedRolesAndAdminAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager)
    {
        const string adminRole = "Admin";
        const string adminEmail = "admin@library.com";
        const string adminPassword = "Admin123!";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to create admin user.");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}