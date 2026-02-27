// Controllers/RecurringExpenseController.cs
// Controller for managing recurring expense templates
// users can create, edit, pause, and delete recurring items
// these templates generate real expenses on the dashboard

using System.Linq; // for OrderBy
using System.Threading.Tasks; // for async
using Microsoft.AspNetCore.Mvc; // controller base
using Microsoft.AspNetCore.Mvc.Rendering; // for SelectList
using Microsoft.EntityFrameworkCore; // EF Core async methods
using BudgetingApp.Data; // database context
using BudgetingApp.Models; // RecurringExpense model

namespace BudgetingApp.Controllers
{
    public class RecurringExpenseController : Controller
    {
        // database context
        private readonly ApplicationDbContext _context;

        // inject context
        public RecurringExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // shows all recurring expense templates
        // includes category info for display
        public async Task<IActionResult> Index()
        {
            var items = await _context.RecurringExpenses
                .Include(r => r.Category)
                .OrderBy(r => r.Name)
                .ToListAsync();

            return View(items);
        }

        // shows details for one recurring item
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.RecurringExpenses
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.RecurringExpenseId == id);

            if (item == null) return NotFound();

            return View(item);
        }

        // shows create form
        // includes category dropdown
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(
                _context.Categories.OrderBy(c => c.Name),
                "CategoryId",
                "Name"
            );

            return View();
        }

        // handles create submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecurringExpenseId,Name,Amount,Interval,NextOccurrenceDate,EndDate,IsActive,CategoryId")] RecurringExpense item)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(
                    _context.Categories.OrderBy(c => c.Name),
                    "CategoryId",
                    "Name",
                    item.CategoryId
                );
                return View(item);
            }

            _context.Add(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // shows edit form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.RecurringExpenses.FindAsync(id);
            if (item == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(
                _context.Categories.OrderBy(c => c.Name),
                "CategoryId",
                "Name",
                item.CategoryId
            );

            return View(item);
        }

        // handles edit submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecurringExpenseId,Name,Amount,Interval,NextOccurrenceDate,EndDate,IsActive,CategoryId")] RecurringExpense item)
        {
            if (id != item.RecurringExpenseId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(
                    _context.Categories.OrderBy(c => c.Name),
                    "CategoryId",
                    "Name",
                    item.CategoryId
                );
                return View(item);
            }

            _context.Update(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // shows delete confirmation
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.RecurringExpenses
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.RecurringExpenseId == id);

            if (item == null) return NotFound();

            return View(item);
        }

        // handles delete confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.RecurringExpenses.FindAsync(id);
            if (item != null) _context.RecurringExpenses.Remove(item);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}