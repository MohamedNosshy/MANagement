using Mangement.Models;
using Mangement.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Mangement.Services
{
    public class TaskService : ITaskService
    {
        private readonly CompContext _context;
        private readonly ILogger<TaskService> _logger;
        private readonly IMemoryCache _cache;

        public TaskService(CompContext context, ILogger<TaskService> logger, IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<EmployeeTask?> GetTaskByIdAsync(int id)
        {
            return await _context.EmployeeTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<EmployeeTask>> GetAllTasksAsync()
        {
            var cacheKey = "AllTasks";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<EmployeeTask> tasks))
            {
                tasks = await _context.EmployeeTasks
                    .Include(t => t.AssignedTo)
                    .Include(t => t.CreatedBy)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, tasks, cacheOptions);
            }
            return tasks;
        }

        public async Task<IEnumerable<EmployeeTask>> GetTasksByUserAsync(int userId)
        {
            return await _context.EmployeeTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Where(t => t.AssignedToId == userId || t.CreatedById == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeTask>> GetTasksByDepartmentAsync(int departmentId)
        {
            return await _context.EmployeeTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Where(t => t.AssignedTo.DepartmentId == departmentId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<EmployeeTask> CreateTaskAsync(EmployeeTask task)
        {
            try
            {
                task.CreatedAt = DateTime.Now;
                task.Status = "Pending";

                _context.EmployeeTasks.Add(task);
                await _context.SaveChangesAsync();

                // Clear cache
                _cache.Remove("AllTasks");
                _cache.Remove("DashboardStats");
                _cache.Remove("RecentActivities");

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        public async Task<EmployeeTask> UpdateTaskAsync(EmployeeTask task)
        {
            try
            {
                var existingTask = await _context.EmployeeTasks.FindAsync(task.Id);
                if (existingTask == null)
                    throw new KeyNotFoundException($"Task with ID {task.Id} not found");

                // Update properties
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.AssignedToId = task.AssignedToId;
                existingTask.StartDate = task.StartDate;
                existingTask.DueDate = task.DueDate;
                existingTask.Priority = task.Priority;
                existingTask.Status = task.Status;

                await _context.SaveChangesAsync();

                // Clear cache
                _cache.Remove("AllTasks");
                _cache.Remove("DashboardStats");
                _cache.Remove("RecentActivities");

                return existingTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                throw;
            }
        }

        public async Task DeleteTaskAsync(int id)
        {
            try
            {
                var task = await _context.EmployeeTasks.FindAsync(id);
                if (task == null)
                    throw new KeyNotFoundException($"Task with ID {id} not found");

                _context.EmployeeTasks.Remove(task);
                await _context.SaveChangesAsync();

                // Clear cache
                _cache.Remove("AllTasks");
                _cache.Remove("DashboardStats");
                _cache.Remove("RecentActivities");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                throw;
            }
        }

        public async Task<EmployeeTask> UpdateTaskStatusAsync(int id, string status)
        {
            try
            {
                var task = await _context.EmployeeTasks.FindAsync(id);
                if (task == null)
                    throw new KeyNotFoundException($"Task with ID {id} not found");

                task.Status = status;
                if (status == "Completed")
                {
                    task.CompletedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                // Clear cache
                _cache.Remove("AllTasks");
                _cache.Remove("DashboardStats");
                _cache.Remove("RecentActivities");

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status");
                throw;
            }
        }

        public async Task<EmployeeTask> AddTaskCommentAsync(int id, string comment)
        {
            try
            {
                var task = await _context.EmployeeTasks.FindAsync(id);
                if (task == null)
                    throw new KeyNotFoundException($"Task with ID {id} not found");

                task.Comments = comment;
                await _context.SaveChangesAsync();

                // Clear cache
                _cache.Remove("AllTasks");
                _cache.Remove("RecentActivities");

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding task comment");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetTaskStatisticsAsync()
        {
            var totalTasks = await _context.EmployeeTasks.CountAsync();
            var pendingTasks = await _context.EmployeeTasks.CountAsync(t => t.Status == "Pending");
            var inProgressTasks = await _context.EmployeeTasks.CountAsync(t => t.Status == "In Progress");
            var completedTasks = await _context.EmployeeTasks.CountAsync(t => t.Status == "Completed");

            return new Dictionary<string, int>
            {
                ["Total"] = totalTasks,
                ["Pending"] = pendingTasks,
                ["InProgress"] = inProgressTasks,
                ["Completed"] = completedTasks
            };
        }
    }
} 