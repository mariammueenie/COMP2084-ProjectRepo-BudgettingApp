// Data/ApplicationDbContext.cs
// Implements:
//      Budgets
//      Recurring expenses
//      Money amounts stored as decimal(10,2) to avoid floating point issues and ensure proper formatting in SQL
// Why:
// EF Core needs DbSet to create tables properly
// Precision avoids rounding errors
// Unique index prevents duplicate budgets

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BudgetingApp.Models;

namespace BudgetingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tables
        public DbSet<Expense> Expenses { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Income> Incomes { get; set; } = default!;
        public DbSet<Budget> Budgets { get; set; } = default!;
        public DbSet<RecurringExpense> RecurringExpenses { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ----------------------------------
            // Seed demo data for first run
            // ----------------------------------

            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Food" },
                new Category { CategoryId = 2, Name = "Rent" },
                new Category { CategoryId = 3, Name = "Transportation" }
            );

            builder.Entity<Income>().HasData(
                new Income
                {
                    IncomeId = 1,
                    Amount = 3200,
                    Date = new DateTime(2026, 3, 1)
                }
            );

            builder.Entity<Expense>().HasData(
                new Expense
                {
                    ExpenseId = 1,
                    Name = "Groceries",
                    Amount = 120,
                    Date = new DateTime(2026, 3, 2),
                    CategoryId = 1
                },
                new Expense
                {
                    ExpenseId = 2,
                    Name = "Bus Pass",
                    Amount = 90,
                    Date = new DateTime(2026, 3, 3),
                    CategoryId = 3
                }
            );

            // ----------------------------------
            // Money precision rules
            // ----------------------------------

            builder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(10, 2);

            builder.Entity<Income>()
                .Property(i => i.Amount)
                .HasPrecision(10, 2);

            builder.Entity<Budget>()
                .Property(b => b.Amount)
                .HasPrecision(10, 2);

            builder.Entity<RecurringExpense>()
                .Property(r => r.Amount)
                .HasPrecision(10, 2);

            // ----------------------------------
            // Prevent duplicate budgets
            // (one budget per category per month)
            // ----------------------------------

            builder.Entity<Budget>()
                .HasIndex(b => new { b.CategoryId, b.Month })
                .IsUnique();
        }
    }
}