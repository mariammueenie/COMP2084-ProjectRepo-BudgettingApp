// Controllers/CategoryController.cs
// Controller for managing categories
// budgets and recurring expenses depend on categories
// basic CRUD so users can create, edit, and delete categories

using System.Linq; // for OrderBy
using System.Threading.Tasks; // for async
using Microsoft.AspNetCore.Mvc; // controller base
using Microsoft.EntityFrameworkCore; // EF Core async methods
using BudgetingApp.Data; // database context
using BudgetingApp.Models; // Category model

namespace BudgetingApp.Controllers
{
    public class CategoryController : Controller
    {
        // database context
        private readonly ApplicationDbContext _context;

        // inject context
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // shows all categories sorted by name
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        // shows details for one category
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // include related expenses
            var category = await _context.Categories
                .Include(c => c.Expense)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();

            return View(category);
        }

        // returns create form
        public IActionResult Create()
        {
            return View();
        }

        // handles create form submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Name")] Category category)
        {
            if (!ModelState.IsValid) return View(category);

            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // returns edit form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // handles edit submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name")] Category category)
        {
            if (id != category.CategoryId) return NotFound();

            if (!ModelState.IsValid) return View(category);

            _context.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // returns delete confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return NotFound();

            return View(category);
        }

        // handles delete confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null) _context.Categories.Remove(category);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}