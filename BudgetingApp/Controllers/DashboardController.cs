// Controllers/DashboardController.cs
// Dashboard controller for the main budgeting overview page
// pulls totals, trends, budgets vs actual, and a simple health score
// also generates recurring expenses so totals stay up to date

using System; // need DateTime and Math helpers
using System.Linq; // need Where, Select, OrderBy
using System.Threading.Tasks; // need async Task
using Microsoft.AspNetCore.Mvc; // need Controller and IActionResult
using Microsoft.EntityFrameworkCore; // need EF Core async queries like SumAsync and ToListAsync
using BudgetingApp.Data; // need ApplicationDbContext
using BudgetingApp.Models; // need models like Expense and RecurringExpense
using BudgetingApp.Models.ViewModels; // need DashboardViewModel and CategoryBudgetRow

namespace BudgetingApp.Controllers
{
    public class DashboardController : Controller
    {
        // database context so controller can query and save data
        private readonly ApplicationDbContext _context;

        // DI injects the db context when controller is created
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // main dashboard page
        // month is optional so user can view different months
        public async Task<IActionResult> Index(DateTime? month)
        {
            // month picker uses yyyy-MM
            // normalize to first day of that month so comparisons are consistent
            var selectedMonth = new DateTime(
                (month ?? DateTime.Today).Year,
                (month ?? DateTime.Today).Month,
                1
            );

            // generate any due recurring expenses before totals so numbers are current
            await GenerateDueRecurringExpensesAsync();

            // date range for the selected month
            var monthStart = selectedMonth;
            var monthEnd = selectedMonth.AddMonths(1);

            // total income for selected month
            // cast to decimal? so SumAsync returns null instead of throwing on empty set
            var totalIncome = await _context.Incomes
                .Where(i => i.Date >= monthStart && i.Date < monthEnd)
                .SumAsync(i => (decimal?)i.Amount) ?? 0m;

            // total expenses for selected month
            var totalExpenses = await _context.Expenses
                .Where(e => e.Date >= monthStart && e.Date < monthEnd)
                .SumAsync(e => (decimal?)e.Amount) ?? 0m;

            // build the viewmodel for the dashboard
            var vm = new DashboardViewModel
            {
                SelectedMonth = selectedMonth,
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                Net = totalIncome - totalExpenses
            };

            // 6 month trend chart including selected month
            // start 5 months back so total is 6 months
            var start = selectedMonth.AddMonths(-5);
            for (int k = 0; k < 6; k++)
            {
                var m = start.AddMonths(k);
                var s = new DateTime(m.Year, m.Month, 1);
                var e = s.AddMonths(1);

                // label for chart x axis
                vm.MonthLabels.Add(m.ToString("MMM yyyy"));

                // income for that month
                var incomeM = await _context.Incomes
                    .Where(x => x.Date >= s && x.Date < e)
                    .SumAsync(x => (decimal?)x.Amount) ?? 0m;

                // expenses for that month
                var expenseM = await _context.Expenses
                    .Where(x => x.Date >= s && x.Date < e)
                    .SumAsync(x => (decimal?)x.Amount) ?? 0m;

                // push values into the chart series lists
                vm.IncomeSeries.Add(incomeM);
                vm.ExpenseSeries.Add(expenseM);
            }

            // budgets vs actual by category
            // ordered so table looks consistent every time
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            foreach (var cat in categories)
            {
                // budget for this category in selected month
                // default 0 if no budget is set
                var budget = await _context.Budgets
                    .Where(b => b.CategoryId == cat.CategoryId && b.Month == selectedMonth)
                    .Select(b => (decimal?)b.Amount)
                    .FirstOrDefaultAsync() ?? 0m;

                // total spent for this category in selected month
                var spent = await _context.Expenses
                    .Where(e => e.CategoryId == cat.CategoryId && e.Date >= monthStart && e.Date < monthEnd)
                    .SumAsync(e => (decimal?)e.Amount) ?? 0m;

                // percent used for progress bar
                // clamp to 100 so bar doesnt overflow in UI
                var pct = budget <= 0 ? 0 : Math.Min(100m, (spent / budget) * 100m);

                // warnings based on budget usage
                // 85 percent is a warning zone
                var status = "OK";
                if (budget > 0 && spent >= budget) status = "OverBudget";
                else if (budget > 0 && spent >= budget * 0.85m) status = "NearLimit";

                // add row for this category to the table
                vm.CategoryBudgets.Add(new CategoryBudgetRow
                {
                    CategoryName = cat.Name,
                    BudgetAmount = budget,
                    SpentAmount = spent,
                    PercentUsed = pct,
                    Status = status
                });
            }

            // compute a simple health score for portfolio dashboard metric
            vm.HealthScore = ComputeHealthScore(vm.TotalIncome, vm.TotalExpenses, vm.CategoryBudgets);

            // label for UI so score is understandable at a glance
            vm.HealthLabel = vm.HealthScore switch
            {
                >= 85 => "Strong",
                >= 70 => "Good",
                >= 50 => "Needs Attention",
                _ => "High Risk"
            };

            return View(vm);
        }

        // turns monthly data into a 0 to 100 score
        // not financial advice, just a simple metric for the app
        private int ComputeHealthScore(decimal income, decimal expenses, System.Collections.Generic.List<CategoryBudgetRow> rows)
        {
            // if no income, score stays low because budgeting is unstable without income
            if (income <= 0) return 30;

            // savings rate = leftover income percentage
            var savingsRate = (income - expenses) / income;

            // savingsPoints is capped so savings doesnt dominate the whole score
            var savingsPoints = (int)Math.Clamp(savingsRate * 60m, 0m, 60m);

            // count how many categories are over or near budget
            var over = rows.Count(r => r.Status == "OverBudget");
            var near = rows.Count(r => r.Status == "NearLimit");

            // penalties push score down when spending is out of control
            var penalty = over * 12 + near * 6;

            // base score + savings - penalties
            var score = 40 + savingsPoints - penalty;

            return (int)Math.Clamp(score, 0, 100);
        }

        // generates Expense rows from recurring templates when they are due
        // runs when dashboard loads so totals reflect recurring bills
        private async Task GenerateDueRecurringExpensesAsync()
        {
            var today = DateTime.Today;

            // pull recurring templates that are active and due
            // also respect EndDate if user set one
            var due = await _context.RecurringExpenses
                .Where(r =>
                    r.IsActive &&
                    r.NextOccurrenceDate <= today &&
                    (r.EndDate == null || r.EndDate >= today))
                .ToListAsync();

            // nothing due, nothing to do
            if (due.Count == 0) return;

            foreach (var r in due)
            {
                // create a real expense from template
                _context.Expenses.Add(new Expense
                {
                    Name = r.Name + " (Recurring)",
                    Amount = r.Amount,
                    Date = r.NextOccurrenceDate,
                    CategoryId = r.CategoryId
                });

                // move the next occurrence forward based on interval
                r.NextOccurrenceDate = r.Interval switch
                {
                    RecurrenceInterval.Weekly => r.NextOccurrenceDate.AddDays(7),
                    RecurrenceInterval.Monthly => r.NextOccurrenceDate.AddMonths(1),
                    _ => r.NextOccurrenceDate.AddMonths(1)
                };
            }

            // saves both the new expenses and the updated NextOccurrenceDate values
            await _context.SaveChangesAsync();
        }
    }
}