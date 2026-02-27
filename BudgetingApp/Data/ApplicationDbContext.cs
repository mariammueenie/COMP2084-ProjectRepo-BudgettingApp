// Data/ApplicationDbContext.cs
// Implements:
//      Budgets
//      Recurring expenses
//      Money amounts stored as decimal(10,2) to avoid floating point issues and ensure proper formatting in SQL
// Why:
// EF core needs DBset to make tables properly    
// Precision avoids rounding errors 
// Unique index prevents duplicates


using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// Enables ASP.NET identity features (like user management, authentication, security features, etc.)

using Microsoft.EntityFrameworkCore;
// Provides classes and methods for interacting with databases using Entity Framework Core (EF Core).

using BudgetingApp.Models;
// Imports models defined in Models.

namespace BudgetingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Existing tables
        public DbSet<Expense> Expenses { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Income> Incomes { get; set; } = default!;

        // new tables
        public DbSet<Budget> Budgets { get; set; } = default!;
        public DbSet<RecurringExpense> RecurringExpenses { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Money precision
            modelBuilder.Entity<Expense>().Property(e => e.Amount).HasPrecision(10, 2);
            modelBuilder.Entity<Income>().Property(i => i.Amount).HasPrecision(10, 2);
            modelBuilder.Entity<Budget>().Property(b => b.Amount).HasPrecision(10, 2);
            modelBuilder.Entity<RecurringExpense>().Property(r => r.Amount).HasPrecision(10, 2);

            // Prevent duplicates: only 1 budget per category per month
            modelBuilder.Entity<Budget>()
                .HasIndex(b => new { b.CategoryId, b.Month })
                .IsUnique();
        }
    }
}
