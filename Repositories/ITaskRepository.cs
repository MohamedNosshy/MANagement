using Mangement.Models;

namespace Mangement.Repositories
{
    public interface ITaskRepository
    {
        Task<EmployeeTask> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeTask>> GetAllAsync();
        Task<IEnumerable<EmployeeTask>> GetByUserAsync(int userId);
        Task<IEnumerable<EmployeeTask>> GetByDepartmentAsync(int departmentId);
        Task<EmployeeTask> CreateAsync(EmployeeTask task);
        Task<EmployeeTask> UpdateAsync(EmployeeTask task);
        Task DeleteAsync(int id);
    }
} 