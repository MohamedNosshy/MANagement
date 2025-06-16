using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mangement.Models
{
    public class Leave
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "نوع الإجازة مطلوب")]
        public string Type { get; set; } // سنوية، مرضية، طارئة
        
        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        public DateTime StartDate { get; set; }
        
        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        public DateTime EndDate { get; set; }
        
        [Required(ErrorMessage = "السبب مطلوب")]
        public string Reason { get; set; }
        
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string? RejectionReason { get; set; }
    }
} 