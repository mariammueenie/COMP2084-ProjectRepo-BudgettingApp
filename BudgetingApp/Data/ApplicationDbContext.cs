

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// Enables ASP.NET identity features (like user management, authentication, security features, etc.)

using Microsoft.EntityFrameworkCore;
// Provides classes and methods for interacting with databases using Entity Framework Core (EF Core).

using BudgetingApp.Models;
// Imports models defined in Models.

namespace BudgetingApp.Data
{
    // This class manages connection to database using EF Core. 
    // Tracks models and turns them into database tables. 

    public class ApplicationDbContext : IdentityDbContext // Inherits identity tables (users, roles, etc.)
    {
        // Constructor that takes DbContextOptions to pass options to base class.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        {
            // just chilling for now
        }

        // This line tells EF Core to create a table for the Expense Model.
        public DbSet<Expense> Expenses { get; set; }

        // This line tells EF Core to create a table for the Category Model.
        public DbSet<Category> Categories { get; set; }

        // This line tells EF Core to create a table for the Budget Model.
        // public DbSet<Budget> Budgets { get; set; }

        // Potential optional override method if you want to configure model further, later on.
    }
}
