
using System;
using System.ComponentModel.DataAnnotations;

namespace BudgetingApp.Models
{
    public class Income
    {
        public int IncomeId { get; set; }

        [Required]
        [Display(Name = "Source of Income")]
        public string Source { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000000)]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Received")]
        public DateTime Date { get; set; }
    }
}
