// Models/ViewModels/DashboardViewModel.cs
// ViewModel used to send structured data to dashboard view
// instead of passing randomly, everything dashboard needs is grouped here
// keeps controller clean, view strongly typed

using System // need DateTime
using System.Collections.Generic // need List collections

namespace BudgetingApp.Models.ViewModels // separate namespace for ViewModels to keep architecture clean
{
    public class DashboardViewModel
    {
        // month currently viewed on the dashboard
        // totals and charts are calculated based on the month
        public DateTime SelectedMonth { get; set; }

        // High-level financial summary for selected month
        // values calculated in controller
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Net { get; set; }
        // Net = Income - Expenses
        // Stored directly to avoid recalculating inside view

        // trend chart data for last 6 months
        // lists used because chart libraries expect arrays of values
        public List<string> MonthLabels { get; set; } = new();
        // example: ["Sep", "Oct", "Nov", "Dec", "Jan", "Feb"]

        public List<decimal> IncomeSeries { get; set; } = new();
        public List<decimal> ExpenseSeries { get; set; } = new();
        // separate lists make it easy to plug into chart

        // budget comp. rows per category
        // supports showing budget vs. actual spending
        public List<CategoryBudgetRow> CategoryBudgets { get; set; } = new();

        // Overall financial health score
        // Int keeps it simple and easy to calculate from rules
        public int HealthScore { get; set; }

        // Label version of health score for UI display
        // Example: "Good", "Warning", "Critical"
        public string HealthLabel { get; set; } = "OK";
    }

    // Keeps category-specific calculations grouped together
    public class CategoryBudgetRow
    {
        // name of category (ex: Groceries, Rent)
        public string CategoryName { get; set; } = "";

        // Budgeted amount for month
        public decimal BudgetAmount { get; set; }

        // Actual amount spent in month
        public decimal SpentAmount { get; set; }

        // Percentage of budget used
        // Calculated in controller so view displays it
        public decimal PercentUsed { get; set; }

        // Status used for UI styling
        // Example: OK, NearLimit, OverBudget
        // Makes it easier to apply color logic in view
        public string Status { get; set; } = "OK";
    }
}