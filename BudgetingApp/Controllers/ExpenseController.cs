// Controllers/ExpenseController.cs
// New version compared to old version
// old: Index showed everything with no filters and got messy as data grows
// old: version used only basic CRUD actions
// old: version had temporary Console.WriteLine debugging in Edit
// new: version removes debugging and keeps Edit clean
// new: version adds ExportCsv so user can export the same filtered list they are viewing
// new: version adds QuickAdd for modal submit so user can add without leaving page
// new: Index takes filter params and returns a ViewModel that keeps filters + results together

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetingApp.Data;
using BudgetingApp.Models;
using BudgetingApp.Models.ViewModels;

namespace BudgetingApp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expense
        // old: Index had no parameters and always returned all rows
        // new: Index accepts filter inputs so the list stays usable
        // new: Index uses ExpenseIndexViewModel so filters persist after submit
        public async Task<IActionResult> Index(
            DateTime? from,
            DateTime? to,
            int? categoryId,
            decimal? minAmount,
            decimal? maxAmount,
            string? search)
        {
            // old version only passed a list of expenses to the view
            // new version passes viewmodel with both filters and results
            var vm = new ExpenseIndexViewModel
            {
                From = from,
                To = to,
                CategoryId = categoryId,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
                Search = search
            };

            // old version only built category dropdown in Create/Edit views
            // new version also builds dropdown options for filtering on Index
            vm.CategoryOptions = await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                .ToListAsync();

            // old: version did not shape queries, it just loaded everything
            // new: version starts with a query and applies filters only if user supplied them
            var q = _context.Expenses.Include(e => e.Category).AsQueryable();

            // new filters
            if (from.HasValue) q = q.Where(e => e.Date >= from.Value.Date);
            if (to.HasValue) q = q.Where(e => e.Date <= to.Value.Date);
            if (categoryId.HasValue) q = q.Where(e => e.CategoryId == categoryId.Value);
            if (minAmount.HasValue) q = q.Where(e => e.Amount >= minAmount.Value);
            if (maxAmount.HasValue) q = q.Where(e => e.Amount <= maxAmount.Value);

            // old: version had no search feature
            // new: version adds basic name search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(e => e.Name.Contains(s));
            }

            // old: version returned items unsorted
            // new: version sorts newest first for better UX
            vm.Expenses = await q
                .OrderByDescending(e => e.Date)
                .ThenByDescending(e => e.ExpenseId)
                .ToListAsync();

            return View(vm);
        }

        // new: action that old controller did not have
        // export matches the same filters as Index so user exports what they see
        [HttpGet]
        public async Task<IActionResult> ExportCsv(
            DateTime? from,
            DateTime? to,
            int? categoryId,
            decimal? minAmount,
            decimal? maxAmount,
            string? search)
        {
            // same filter logic as Index so export respects UI state
            var q = _context.Expenses.Include(e => e.Category).AsQueryable();

            if (from.HasValue) q = q.Where(e => e.Date >= from.Value.Date);
            if (to.HasValue) q = q.Where(e => e.Date <= to.Value.Date);
            if (categoryId.HasValue) q = q.Where(e => e.CategoryId == categoryId.Value);
            if (minAmount.HasValue) q = q.Where(e => e.Amount >= minAmount.Value);
            if (maxAmount.HasValue) q = q.Where(e => e.Amount <= maxAmount.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(e => e.Name.Contains(s));
            }

            var rows = await q.OrderByDescending(e => e.Date).ToListAsync();

            // old: controller had no export feature
            // new: controller builds a csv file in memory and returns it as a download
            var sb = new StringBuilder();
            sb.AppendLine("Date,Name,Category,Amount");

            foreach (var e in rows)
            {
                // basic csv escaping so commas and quotes do not break rows
                static string Esc(string v) => "\"" + v.Replace("\"", "\"\"") + "\"";
                sb.AppendLine($"{e.Date:yyyy-MM-dd},{Esc(e.Name)},{Esc(e.Category?.Name ?? "")},{e.Amount:0.00}");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "expenses.csv");
        }

        // new: action that old controller did not have
        // quick add is for ajax modal so user can add without leaving Index page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickAdd([Bind("Name,Amount,Date,CategoryId")] Expense expense)
        {
            // old version required navigating to Create page
            // new version stays on Index and returns ok or bad request for the modal ui
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Add(expense);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        // CRUD below stays mostly the same as old version
        // main differences are Index features and the removed debug prints

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.ExpenseId == id);

            if (expense == null) return NotFound();

            return View(expense);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpenseId,Name,Amount,Date,CategoryId")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", expense.CategoryId);
            return View(expense);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", expense.CategoryId);
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpenseId,Name,Amount,Date,CategoryId")] Expense expense)
        {
            // old: version included Console.WriteLine debugging here
            // new: version keeps it clean
            if (id != expense.ExpenseId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", expense.CategoryId);
            return View(expense);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.ExpenseId == id);

            if (expense == null) return NotFound();

            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // old: version had extra null checks for the DbSet
            // new: version keeps the flow simple
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null) _context.Expenses.Remove(expense);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}