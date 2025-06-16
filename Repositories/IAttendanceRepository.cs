using Mangement.Models;

namespace Mangement.Repositories
{
    public interface IAttendanceRepository
    {
        Task<Attendance> GetByIdAsync(int id);
        Task<IEnumerable<Attendance>> GetAllAsync();
        Task<IEnumerable<Attendance>> GetByUserAsync(int userId);
        Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date);
        Task<Attendance> CreateAsync(Attendance attendance);
        Task<Attendance> UpdateAsync(Attendance attendance);
        Task DeleteAsync(int id);
    }
} 