using Mangement.Models;

namespace Mangement.Repositories
{
    public interface ILeaveRepository
    {
        Task<Leave> GetByIdAsync(int id);
        Task<IEnumerable<Leave>> GetAllAsync();
        Task<IEnumerable<Leave>> GetByUserAsync(int userId);
        Task<IEnumerable<Leave>> GetByStatusAsync(string status);
        Task<Leave> CreateAsync(Leave leave);
        Task<Leave> UpdateAsync(Leave leave);
        Task DeleteAsync(int id);
    }
} 