using Microsoft.EntityFrameworkCore;

namespace Mangement.Models
{
	public class CompContext : DbContext
	{
		public CompContext(DbContextOptions<CompContext> options) : base(options)
		{
		}
		public DbSet<User> Users { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Attendance> Attendances { get; set; }
		public DbSet<Leave> Leaves { get; set; }
		public DbSet<EmployeeTask> EmployeeTasks { get; set; }
		public DbSet<Salary> Salaries { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// تكوين العلاقات
			modelBuilder.Entity<User>()
				.HasOne(u => u.Department)
				.WithMany(d => d.Users)
				.HasForeignKey(u => u.DepartmentId);

			modelBuilder.Entity<Attendance>()
				.HasOne(a => a.User)
				.WithMany(u => u.Attendances)
				.HasForeignKey(a => a.UserId);

			modelBuilder.Entity<Leave>()
				.HasOne(l => l.User)
				.WithMany(u => u.Leaves)
				.HasForeignKey(l => l.UserId);

			modelBuilder.Entity<EmployeeTask>()
				.HasOne(t => t.AssignedTo)
				.WithMany(u => u.AssignedTasks)
				.HasForeignKey(t => t.AssignedToId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<EmployeeTask>()
				.HasOne(t => t.CreatedBy)
				.WithMany(u => u.CreatedTasks)
				.HasForeignKey(t => t.CreatedById)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Salary>()
				.HasOne(s => s.User)
				.WithMany(u => u.Salaries)
				.HasForeignKey(s => s.UserId);

			// تحديد اسم الجدول للمهام
			modelBuilder.Entity<EmployeeTask>().ToTable("EmployeeTasks");

			// تحديد دقة الأرقام العشرية
			modelBuilder.Entity<Salary>()
				.Property(s => s.BaseSalary)
				.HasColumnType("decimal(18,2)");
			modelBuilder.Entity<Salary>()
				.Property(s => s.Allowances)
				.HasColumnType("decimal(18,2)");
			modelBuilder.Entity<Salary>()
				.Property(s => s.Deductions)
				.HasColumnType("decimal(18,2)");
			modelBuilder.Entity<Salary>()
				.Property(s => s.TotalSalary)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<User>()
				.Property(u => u.BaseSalary)
				.HasColumnType("decimal(18,2)");
		}
	}
}
