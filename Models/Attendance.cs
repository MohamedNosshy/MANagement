using System.ComponentModel.DataAnnotations.Schema;

namespace Mangement.Models
{
	public class Attendance
	{
		public int Id { get; set; }
		public DateTime InDate { get; set; }
		public DateTime OutDate { get; set; }
		
		[ForeignKey("User")]
		public int UserId { get; set; }
		public User User { get; set; } 
	}
}
