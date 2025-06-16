using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mangement.Models
{
    public class Salary
    {
        public int Id { get; set; }
        
        [Required]
        public decimal BaseSalary { get; set; }
        
        public decimal? Allowances { get; set; }
        public decimal? Deductions { get; set; }
        
        public decimal TotalSalary { get; set; }
        
        public DateTime PaymentDate { get; set; }
        
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        
        public string? Notes { get; set; }
        
        public bool IsPaid { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 