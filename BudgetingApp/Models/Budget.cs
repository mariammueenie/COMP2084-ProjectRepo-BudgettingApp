// Models/Budget.cs
// Purpose:
// Defines the "Budget" entity that will be stored in the database via Entity Framework Core.
// Implements feature 1A: monthly budget per category (ex: Groceries $500 for 2026-02).

using System; // Needed for DateTime (used by the Month property).
using System.ComponentModel.DataAnnotations; // Provides validation attributes like [Required], [Range], [DataType].
using System.ComponentModel.DataAnnotations.Schema; // Provides mapping attributes like [Column] for database column types.

// The "Budget" class represents a monthly budget for a specific category
// It includes properties for:
//      BudgetId: Primary key for the budget entry
//      Month: The month and year the budget applies to (e.g., February 2026)
//      Amount: The budgeted amount for that month and category
//      CategoryId: Foreign key linking to the Category entity
// Data annotations are used to enforce validation rules and specify database column types 

namespace BudgetingApp.Models
{
    // Primary Key: BudgetId
    public class Budget
    {public int BudgetId { get; set; }

    // Month:
    //      Required: A budget must have a month specified
    // Stored as the first day of the month (e.g., 2026-02-01 for February 2026)

    [Required] // Validation:
    // In MVC: ModelState.IsValid will fail if month is missing
    // In DB: Column cannot be null
    [DataType(DataType.Date)]
    public DateTime Month { get; set; }
        // Best practice recommendation: normalize to first-of-month at midnight

    // Amount:
    // - Required: A budget must have an amount specified
    // - Range: Must be between $0.01 and $50,000.00
    //      Forces SQL column to be decimal with 2 decimal places
    //      Prevents EF/SQL from picking a type/precision you didn't intend
    //      Avoids floating-point rounding issues (decimal is correct for money)
    //      (10,2) means: up to 10 total digits, with 2 digits after the decimal
    [Required]
    [Range(0.01, 50000.00)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    // CategoryId:
    // Connects this budget to a specific category
    [Required]
    public int CategoryId { get; set; }

    // Navigation property to the related Category entity
    // This allows us to access the Category details from a Budget instance
    // Marked as nullable (Category?) because EF Core will populate this property when loading related data
    // If you try to access Category without including it in your query, it will be null
    public Category? Category { get; set; }

    }
}