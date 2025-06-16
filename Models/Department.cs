using System.ComponentModel.DataAnnotations;

namespace Mangement.Models
{
	public class Department
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		public virtual List<User> Users { get; set; }
	}
}
