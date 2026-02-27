// Controllers/BudgetController.cs
// Controller for managing monthly budgets
// users can create, edit, and delete budgets
// budgets are stored per category per month

using System; // for DateTime
using System.Linq; // for OrderBy and Where
using System.Threading.Tasks; // for async
using Microsoft.AspNetCore.Mvc; // controller base
using Microsoft.AspNetCore.Mvc.Rendering; // for SelectList
using Microsoft.EntityFrameworkCore; // EF Core async methods
using BudgetingApp.Data; // database context
using BudgetingApp.Models; // Budget model

namespace BudgetingApp.Controllers
{
    public class BudgetController : Controller
    {
        // database context
        private readonly ApplicationDbContext _context;

        // inject context
        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // shows budgets for selected month
        // month defaults to current month
        public async Task<IActionResult> Index(DateTime? month)
        {
            // normalize to first day of month
            var selectedMonth = new DateTime(
                (month ?? DateTime.Today).Year,
                (month ?? DateTime.Today).Month,
                1
            );

            ViewBag.SelectedMonth = selectedMonth;

            // load budgets for that month with category info
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.Month == selectedMonth)
                .OrderBy(b => b.Category!.Name)
                .ToListAsync();

            return View(budgets);
        }

        // shows details for one budget
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BudgetId == id);

            if (budget == null) return NotFound();

            return View(budget);
        }

        // shows create form
        public async Task<IActionResult> Create()
        {
            // dropdown for categories
            ViewData["CategoryId"] = new SelectList(
                _context.Categories.OrderBy(c => c.Name),
                "CategoryId",
                "Name"
            );

            // default month is current month
            ViewBag.DefaultMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            return View();
        }

        // handles create submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetId,Month,Amount,CategoryId")] Budget budget)
        {
            // always store month as first day
            budget.Month = new DateTime(budget.Month.Year, budget.Month.Month, 1);

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(
                    _context.Categories.OrderBy(c => c.Name),
                    "CategoryId",
                    "Name",
                    budget.CategoryId
                );
                return View(budget);
            }

            try
            {
                _context.Add(budget);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // duplicate month + category will fail because of unique index
                ModelState.AddModelError("", "Budget already exists for this category and month");
                ViewData["CategoryId"] = new SelectList(
                    _context.Categories.OrderBy(c => c.Name),
                    "CategoryId",
                    "Name",
                    budget.CategoryId
                );
                return View(budget);
            }

            return RedirectToAction(nameof(Index), new { month = budget.Month });
        }

        // shows edit form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(
                _context.Categories.OrderBy(c => c.Name),
                "CategoryId",
                "Name",
                budget.CategoryId
            );

            return View(budget);
        }

        // handles edit submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BudgetId,Month,Amount,CategoryId")] Budget budget)
        {
            if (id != budget.BudgetId) return NotFound();

            // normalize month again
            budget.Month = new DateTime(budget.Month.Year, budget.Month.Month, 1);

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(
                    _context.Categories.OrderBy(c => c.Name),
                    "CategoryId",
                    "Name",
                    budget.CategoryId
                );
                return View(budget);
            }

            try
            {
                _context.Update(budget);
                await _context.SaveChangesAsync();
            }
            catch
            {
                ModelState.AddModelError("", "Could not save budget");
                ViewData["CategoryId"] = new SelectList(
                    _context.Categories.OrderBy(c => c.Name),
                    "CategoryId",
                    "Name",
                    budget.CategoryId
                );
                return View(budget);
            }

            return RedirectToAction(nameof(Index), new { month = budget.Month });
        }

        // shows delete confirmation
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BudgetId == id);

            if (budget == null) return NotFound();

            return View(budget);
        }

        // handles delete confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget != null) _context.Budgets.Remove(budget);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}