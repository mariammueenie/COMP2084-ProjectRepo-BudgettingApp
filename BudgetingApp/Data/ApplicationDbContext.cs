

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
        : base(options)
        {
            // just chilling for now
        }

        // This line tells EF Core to create a table for the Expense Model.
        public DbSet<Expense> Expenses { get; set; }

        // This line tells EF Core to create a table for the Category Model.
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Expense>()
        .Property(e => e.Amount)
        .HasPrecision(10, 2); // Total 10 digits, 2 after decimal

    modelBuilder.Entity<Income>()
        .Property(i => i.Amount)
        .HasPrecision(10, 2); // Total 10 digits, 2 after decimal
}


        // this line is for the income model
public DbSet<Income> Incomes { get; set; }
        // This line tells EF Core to create a table for the Income Model.

        // This line tells EF Core to create a table for the Budget Model.
        // public DbSet<Budget> Budgets { get; set; }

        // Potential optional override method if you want to configure model further, later on.
    }
}
