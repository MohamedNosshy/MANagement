using Mangement.Models;

namespace Mangement.Repositories
{
    public interface ISalaryRepository
    {
        Task<Salary> GetByIdAsync(int id);
        Task<IEnumerable<Salary>> GetAllAsync();
        Task<IEnumerable<Salary>> GetByUserAsync(int userId);
        Task<IEnumerable<Salary>> GetByMonthAsync(int year, int month);
        Task<Salary> CreateAsync(Salary salary);
        Task<Salary> UpdateAsync(Salary salary);
        Task DeleteAsync(int id);
    }
} 