using Mangement.Models;

namespace Mangement.Interfaces
{
    public interface ITaskService
    {
        Task<EmployeeTask?> GetTaskByIdAsync(int id);
        Task<IEnumerable<EmployeeTask>> GetAllTasksAsync();
        Task<IEnumerable<EmployeeTask>> GetTasksByUserAsync(int userId);
        Task<IEnumerable<EmployeeTask>> GetTasksByDepartmentAsync(int departmentId);
        Task<EmployeeTask> CreateTaskAsync(EmployeeTask task);
        Task<EmployeeTask> UpdateTaskAsync(EmployeeTask task);
        Task DeleteTaskAsync(int id);
        Task<EmployeeTask> UpdateTaskStatusAsync(int id, string status);
        Task<EmployeeTask> AddTaskCommentAsync(int id, string comment);
    }
} 