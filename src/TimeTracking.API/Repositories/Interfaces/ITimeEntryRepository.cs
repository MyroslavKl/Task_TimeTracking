using TimeTracking.API.Models;

namespace TimeTracking.API.Repositories.Interfaces
{
    public interface ITimeEntryRepository
    {
        Task<IEnumerable<TimeEntry>> GetAllAsync(string? employeeName, CancellationToken ct = default);
        Task<TimeEntry?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(TimeEntry timeEntry, CancellationToken ct = default);
        Task UpdateAsync(TimeEntry timeEntry, CancellationToken ct = default);
        Task<decimal> GetTotalHoursForDayAsync(string employeeName, DateTime date, CancellationToken ct = default);
    }
}
