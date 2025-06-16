using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mangement.Models
{
	public class User
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "الاسم مطلوب")]
		public string Name { get; set; }
		[Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
		[EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
		public string Email { get; set; }
		[Required(ErrorMessage = "كلمة المرور مطلوبة")]
		[DataType(DataType.Password)]
		[MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
		public string Password { get; set; }
		public bool isAdmin { get; set; } = false;
		[ForeignKey("Department")]
		public int DepartmentId { get; set; }
		public Department Department { get; set; }
		
		// العلاقات الجديدة
		public virtual List<Leave> Leaves { get; set; }
		public virtual List<EmployeeTask> AssignedTasks { get; set; }
		public virtual List<EmployeeTask> CreatedTasks { get; set; }
		public virtual List<Salary> Salaries { get; set; }
		public virtual List<Attendance> Attendances { get; set; }
		
		// معلومات إضافية
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public DateTime HireDate { get; set; } = DateTime.Now;
		public string? Position { get; set; }
		public decimal? BaseSalary { get; set; }
	}
}
