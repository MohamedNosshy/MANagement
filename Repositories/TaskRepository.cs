using Microsoft.EntityFrameworkCore;
using Mangement.Models;

namespace Mangement.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly CompContext _context;

        public TaskRepository(CompContext context)
        {
            _context = context;
        }

        public async Task<EmployeeTask> GetByIdAsync(int id)
        {
            return await _context.EmployeeTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<EmployeeTask>> GetAllAsync()
        {
            return await _context.EmployeeTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeTask>> GetByUserAsync(int userId)
        {
            return await _context.EmployeeTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Where(t => t.AssignedToId == userId || t.CreatedById == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeTask>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.EmployeeTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Where(t => t.AssignedTo.DepartmentId == departmentId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<EmployeeTask> CreateAsync(EmployeeTask task)
        {
            _context.EmployeeTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<EmployeeTask> UpdateAsync(EmployeeTask task)
        {
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _context.EmployeeTasks.FindAsync(id);
            if (task != null)
            {
                _context.EmployeeTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
} 