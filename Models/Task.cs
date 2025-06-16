using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mangement.Models
{
    public class EmployeeTask
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "عنوان المهمة مطلوب")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "وصف المهمة مطلوب")]
        public string Description { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        
        public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed, Cancelled
        
        public string Priority { get; set; } = "Medium"; // Low, Medium, High
        
        [ForeignKey("User")]
        public int AssignedToId { get; set; }
        public User AssignedTo { get; set; }
        
        [ForeignKey("User")]
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        
        public string? Comments { get; set; }
    }
} 