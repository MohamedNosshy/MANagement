using System.ComponentModel.DataAnnotations;
using Mangement.Models;

namespace Mangement.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Assigned user is required")]
        public int AssignedToId { get; set; }

        public string Status { get; set; } = "Pending";
        public string Comments { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int CreatedById { get; set; }

        // Navigation properties
        public User? AssignedTo { get; set; }
        public User? CreatedBy { get; set; }

        // Additional properties for the view
        public IEnumerable<User> AvailableUsers { get; set; } = new List<User>();
    }
} 