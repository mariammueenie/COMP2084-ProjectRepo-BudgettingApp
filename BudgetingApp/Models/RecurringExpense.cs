// Models/RecurringExpense.cs
// This model represents a recurring expense template (like Rent or Spotify)
// Instead of manually adding the same expense every week/month,
// this lets the app auto-generate real Expense records when they’re due

using System; // Needed for DateTime
using System.ComponentModel.DataAnnotations; // Gives us validation attributes like [Required], [Range], etc
using System.ComponentModel.DataAnnotations.Schema; // Lets us control how things are stored in the database

namespace BudgetingApp.Models // Organizing this inside the Models folder (standard MVC structure)
{
    // Enum to control how often the recurring expense repeats
    // Using an enum keeps it clean instead of using random strings
    public enum RecurrenceInterval
    {
        Weekly = 1,   // Happens every week
        Monthly = 2   // Happens every month
    }

    public class RecurringExpense
    {
        // Primary key (unique ID for each recurring expense template)
        // EF Core automatically recognizes this as the table key
        public int RecurringExpenseId { get; set; }

        // Name of the recurring expense (ex: "Rent", "Netflix")
        [Required] // User must enter a name
        [StringLength(120)] // Prevents super long names
        [Display(Name = "Name")] // What shows up in forms
        public string Name { get; set; } = string.Empty;
        // Default empty string avoids null issues

        // The fixed amount this recurring expense will generate
        [Required] // Must have a value
        [Range(0.01, 500000)] // Prevents zero or negative amounts
        [Column(TypeName = "decimal(10,2)")]
        // Makes sure money is stored properly (2 decimal places, no floating point errors)
        public decimal Amount { get; set; }

        // Controls whether this repeats weekly or monthly
        [Required] // User must choose one
        [Display(Name = "Interval")] // Friendly label in the UI
        public RecurrenceInterval Interval { get; set; }

        // This is the next date when the app should generate a real Expense record
        // This makes the recurring logic predictable and easy to calculate
        [Required] // Needed so the system knows when to trigger it
        [DataType(DataType.Date)] // Renders as a date picker in forms
        [Display(Name = "Next Occurrence")]
        public DateTime NextOccurrenceDate { get; set; }

        // Optional end date (for things like limited subscriptions)
        // Nullable because some expenses don’t have an end
        [DataType(DataType.Date)]
        [Display(Name = "End Date (optional)")]
        public DateTime? EndDate { get; set; }

        // Allows the user to pause a recurring expense without deleting it
        // This is better UX than forcing deletion
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
        // Default is true because most recurring expenses start active

        // Foreign key to connect this recurring expense to a Category
        // Every recurring expense must belong to some category (like Housing, Food, etc)
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Navigation property
        // Lets me access related category info in code (like recurringExpense.Category.Name)
        // EF Core uses this to manage relationships automatically
        public Category? Category { get; set; }
    }
}