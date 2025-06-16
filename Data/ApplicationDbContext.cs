using Microsoft.EntityFrameworkCore;
using Mangement.Models;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "HR" },
                new Department { Id = 3, Name = "Finance" }
            );

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "John Doe", Email = "john@example.com", Password = "hashed_password_here", DepartmentId = 1 },
                new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Password = "hashed_password_here", DepartmentId = 2 },
                new User { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", Password = "hashed_password_here", DepartmentId = 3 }
            );
        }
    }
} 