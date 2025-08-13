using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetingApp.Data;
using BudgetingApp.Models;

namespace BudgetingApp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Injecting the database context into the controller
        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expense
        // Lists all expenses with their associated categories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Expenses.Include(e => e.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Expense/Details/5
        // Shows the full details of a single expense
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Expenses == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.ExpenseId == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expense/Create
        // Renders the form to create a new expense
        public IActionResult Create()
        {
            // Populates the dropdown list for Category
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Expense/Create
        // Processes the form submission to create a new expense
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

            // Re-populate dropdown if form validation fails
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", expense.CategoryId);
            return View(expense);
        }

        // GET: Expense/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Expenses == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", expense.CategoryId);
            return View(expense);
        }

        // POST: Expense/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpenseId,Name,Amount,Date,CategoryId")] Expense expense)
        {
            if (id != expense.ExpenseId)
            {
                return NotFound();
            }

// temp, for troubleshooting, from openstax
Console.WriteLine($"ModelState is valid: {ModelState.IsValid}");
            foreach (var modelStateKey in ModelState.Keys)
            {
                var value = ModelState[modelStateKey];
                if (value != null) // Prevents possible null dereference.
                {
                    foreach (var error in value.Errors)
                    { 
                        Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.ExpenseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", expense.CategoryId);
            return View(expense);
        }

        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Expenses == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.ExpenseId == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Make sure the expenses table is not null.
            if (_context.Expenses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Expenses' is null.");
            }

            // Find the expense using id.
            var expense = await _context.Expenses.FindAsync(id);

            // If null expense exists, remove it. Prevents null reference exceptions.
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }

            // Redirect to updated list of expenses after null deletion.
            return RedirectToAction(nameof(Index));
        }

        // Helper method to check if an expense exists in the database
        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.ExpenseId == id);
        }
    }
}

