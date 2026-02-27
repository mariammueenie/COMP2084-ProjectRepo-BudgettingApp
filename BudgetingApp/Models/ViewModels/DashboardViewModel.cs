// Models/ViewModels/DashboardViewModel.cs
// ViewModel used to send structured data to dashboard view
// instead of passing randomly, everything dashboard needs is grouped here
// keeps controller clean, view strongly typed

using System; // need DateTime
using System.Collections.Generic; // need List collections

namespace BudgetingApp.Models.ViewModels; // file-scoped namespace (C# 10+)

public class DashboardViewModel
{
    // month currently viewed on the dashboard
    // totals and charts are calculated based on the month
    public DateTime SelectedMonth { get; set; }

    // high-level financial summary for selected month
    // values calculated in controller
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }

    // net = income - expenses
    // stored directly to avoid recalculating inside view
    public decimal Net { get; set; }

    // trend chart data for last 6 months
    // lists used because chart libraries expect arrays of values
    public List<string> MonthLabels { get; set; } = new();
    // example: ["Sep", "Oct", "Nov", "Dec", "Jan", "Feb"]

    public List<decimal> IncomeSeries { get; set; } = new();
    public List<decimal> ExpenseSeries { get; set; } = new();
    // separate lists make it easy to plug into chart

    // budget comparison rows per category
    // supports showing budget vs actual spending
    public List<CategoryBudgetRow> CategoryBudgets { get; set; } = new();

    // overall financial health score
    // int keeps it simple and easy to calculate from rules
    public int HealthScore { get; set; }

    // label version of health score for UI display
    // example: OK, Warning, Critical
    public string HealthLabel { get; set; } = "OK";
}

// keeps category-specific calculations grouped together
public class CategoryBudgetRow
{
    // name of category (ex: Groceries, Rent)
    public string CategoryName { get; set; } = "";

    // budgeted amount for month
    public decimal BudgetAmount { get; set; }

    // actual amount spent in month
    public decimal SpentAmount { get; set; }

    // percentage of budget used
    // calculated in controller so view displays it
    public decimal PercentUsed { get; set; }

    // status used for UI styling
    // example: OK, NearLimit, OverBudget
    // makes it easier to apply color logic in view
    public string Status { get; set; } = "OK";
}