// Controllers/BudgetsController.cs
// Purpose:
// Controller for managing monthly budgets.
// Users can create, edit, view, and delete budgets.
// Budgets are stored per category per month.
// UX:
// - Consistent Category dropdown (sorted + pre-selected)
// - Month filtering on Index (defaults to current month)
// - Month normalization (always saved as first day of the month)

using System; // DateTime
using System.Linq; // LINQ (OrderBy, Where, Any)
using System.Threading.Tasks; // async/await
using Microsoft.AspNetCore.Mvc; // Controller, IActionResult
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList (dropdown)
using Microsoft.EntityFrameworkCore; // EF Core async methods
using BudgetingApp.Data; // ApplicationDbContext
using BudgetingApp.Models; // Budget model

namespace BudgetingApp.Controllers
{
    // NOTE:
    // Controller name determines the default Views folder:
    // BudgetsController => Views/Budgets/*
    public class BudgetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Budgets
        // Optional querystring: ?month=2026-03-01 (any date in the month works)
        public async Task<IActionResult> Index(DateTime? month)
        {
            // Normalize to first day of selected month (or current month)
            var selectedMonth = new DateTime(
                (month ?? DateTime.Today).Year,
                (month ?? DateTime.Today).Month,
                1
            );

            // Used by the Index view for the month picker + heading
            ViewBag.SelectedMonth = selectedMonth;

            // Load budgets for selected month, include Category so we can display category names
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.Month == selectedMonth)
                .OrderBy(b => b.Category!.Name)
                .ToListAsync();

            return View(budgets);
        }

        // GET: Budgets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BudgetId == id);

            if (budget == null) return NotFound();

            return View(budget);
        }

        // GET: Budgets/Create
        public IActionResult Create()
        {
            // UX guard: Budgets require Categories, so redirect if none exist
            if (!_context.Categories.Any())
            {
                TempData["Error"] = "Create at least one Category before adding Budgets.";
                return RedirectToAction("Index", "Category");
            }

            // Populate Category dropdown (sorted)
            PopulateCategoryDropdown();

            // Default month used by the Create view month picker
            ViewBag.DefaultMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            return View();
        }

        // POST: Budgets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetId,Month,Amount,CategoryId")] Budget budget)
        {
            // If categories were deleted or never existed, don't allow budgets to be created
            if (!_context.Categories.Any())
            {
                TempData["Error"] = "Create at least one Category before adding Budgets.";
                return RedirectToAction("Index", "Category");
            }

            // Always store month as first day of the selected month
            budget.Month = NormalizeToFirstOfMonth(budget.Month);

            if (!ModelState.IsValid)
            {
                PopulateCategoryDropdown(budget.CategoryId);
                return View(budget);
            }

            try
            {
                _context.Add(budget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { month = budget.Month });
            }
            catch (DbUpdateException)
            {
                // Duplicate Month + CategoryId will fail due to unique index
                ModelState.AddModelError("", "A budget already exists for this category and month.");
                PopulateCategoryDropdown(budget.CategoryId);
                return View(budget);
            }
        }

        // GET: Budgets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null) return NotFound();

            PopulateCategoryDropdown(budget.CategoryId);
            return View(budget);
        }

        // POST: Budgets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BudgetId,Month,Amount,CategoryId")] Budget budget)
        {
            if (id != budget.BudgetId) return NotFound();

            // Always store month as first day of the selected month
            budget.Month = NormalizeToFirstOfMonth(budget.Month);

            if (!ModelState.IsValid)
            {
                PopulateCategoryDropdown(budget.CategoryId);
                return View(budget);
            }

            try
            {
                _context.Update(budget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { month = budget.Month });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BudgetExists(budget.BudgetId)) return NotFound();
                throw;
            }
            catch (DbUpdateException)
            {
                // Could be duplicate unique index OR other DB issue
                ModelState.AddModelError("", "Could not save budget. A budget for this category and month may already exist.");
                PopulateCategoryDropdown(budget.CategoryId);
                return View(budget);
            }
        }

        // GET: Budgets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BudgetId == id);

            if (budget == null) return NotFound();

            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);

            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();

                // After delete, go back to Index for the same month context
                return RedirectToAction(nameof(Index), new { month = budget.Month });
            }

            // If not found, just return to Index default month
            return RedirectToAction(nameof(Index));
        }

        // ---- Helpers ----

        // UX helper: builds the Category dropdown (sorted) and keeps pre-selection consistent
        private void PopulateCategoryDropdown(int? selectedCategoryId = null)
        {
            var categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name", selectedCategoryId);
        }

        // Data integrity helper: ensures Month is always stored as the first day of the month
        private DateTime NormalizeToFirstOfMonth(DateTime anyDateInMonth)
        {
            return new DateTime(anyDateInMonth.Year, anyDateInMonth.Month, 1);
        }

        private bool BudgetExists(int id)
        {
            return _context.Budgets.Any(e => e.BudgetId == id);
        }
    }
}