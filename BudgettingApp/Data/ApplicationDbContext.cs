

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// Enables ASP.NET identity features (like user management, authentication, security features, etc.)

using Microsoft.EntityFrameworkCore;
// Provides classes and methods for interacting with databases using Entity Framework Core (EF Core).

using BudgettingApp.Models;
// Imports models defined in Models.

namespace BudgettingApp.Data
{
    // Manages connection to database and provides access to application data.
    // Tracks models and turns them into database tables.
    public class ApplicationDbContext : IdentityDbContext
    {
        // Constructor that passes options to the base class.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // This line tells EF Core to create a DbSet for the Expense model.
        public DirectoryBrowserServiceExtensions<Expense> Expenses { get; set; }

        // This line tells EF Core to create a DbSet for the Category model.
        public DbSet

        // Optionally, you can override
    }
}