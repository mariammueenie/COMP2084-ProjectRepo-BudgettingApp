// Models/ViewModels/ExpenseIndexViewModel.cs
// ViewModel for expenses index page
// keeps filters and results together so state stays in UI after searching
// helps avoid using ViewBag and keeps page strongly typed

using System; // need DateTime
using System.Collections.Generic; // need List
using Microsoft.AspNetCore.Mvc.Rendering; // need SelectListItem (dropdown options)

namespace BudgetingApp.Models.ViewModels
{
    public class ExpenseIndexViewModel
    {
        // Filter values entered by user

        // Start date filter
        // nullable because user might not choose one
        public DateTime? From { get; set; }

        // End date filter
        public DateTime? To { get; set; }

        // Filter by category
        // nullable so "All categories" is possible
        public int? CategoryId { get; set; }

        // Minimum amount filter
        public decimal? MinAmount { get; set; }

        // Maximum amount filter
        public decimal? MaxAmount { get; set; }

        // Text search filter (ex: description)
        public string? Search { get; set; }

        // Dropdown list for categories
        // populated in controller
        public List<SelectListItem> CategoryOptions { get; set; } = new();

        // Filtered results returned from database
        // view loops over this list to display expenses
        public List<Expense> Expenses { get; set; } = new();
    }
}