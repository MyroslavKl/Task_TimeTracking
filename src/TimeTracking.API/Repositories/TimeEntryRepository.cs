using Dapper;
using TimeTracking.API.Models;
using TimeTracking.API.Repositories.Interfaces;

namespace TimeTracking.API.Repositories
{
    public class TimeEntryRepository(IConfiguration configuration) : BaseRepository(configuration), 
        ITimeEntryRepository
    {
        private const string QUERY_GET_ALL = @"
            select * 
            from TimeEntries
        ";

        private const string QUERY_SORT_BY_NAME = @"
            select * 
            from TimeEntries
            where
                EmployeeName = @EmployeeName
        ";

        private const string QUERY_GET_BY_ID = @"
            select * 
            from TimeEntries
            where
                Id = @Id
        ";

        private const string QUERY_INSERT_RECORD = @"
            insert into TimeEntries
                 (EmployeeName, Date, HoursWorked, Description)
            values 
                 (@EmployeeName, @Date, @HoursWorked, @Description)
            returning Id
        ";

        private const string QUERY_UPDATE_RECORD = @"
            update TimeEntries 
            set 
                EmployeeName = @EmployeeName, 
                Date = @Date, 
                HoursWorked = @HoursWorked, 
                Description = @Description
            where 
                Id = @Id
        ";

        private const string QUERY_GET_TOTAL_HOURS = @"
            select coalesce(sum(HoursWorked), 0) 
            from TimeEntries
            where
                EmployeeName = @EmployeeName 
                and Date::date = @Date::date
        ";

        public async Task<IEnumerable<TimeEntry>> GetAllAsync(string? employeeName, CancellationToken ct = default)
        {
            using var connection = CreateConnection();
            var sql = string.IsNullOrEmpty(employeeName) ? QUERY_GET_ALL : QUERY_SORT_BY_NAME;
            var param = string.IsNullOrEmpty(employeeName) ? null : new { EmployeeName = employeeName };

            return await connection.QueryAsync<TimeEntry>(new CommandDefinition(sql, param, cancellationToken: ct));
        }

        public async Task<TimeEntry?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<TimeEntry>(
                new CommandDefinition(QUERY_GET_BY_ID, new { Id = id }, cancellationToken: ct));
        }

        public async Task<int> CreateAsync(TimeEntry timeEntry, CancellationToken ct = default)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(QUERY_INSERT_RECORD, timeEntry, cancellationToken: ct));
        }

        public async Task UpdateAsync(TimeEntry timeEntry, CancellationToken ct = default)
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync(
                new CommandDefinition(QUERY_UPDATE_RECORD, timeEntry, cancellationToken: ct));
        }

        public async Task<decimal> GetTotalHoursForDayAsync(string employeeName, DateTime date, CancellationToken ct = default)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<decimal>(
                new CommandDefinition(QUERY_GET_TOTAL_HOURS, new { EmployeeName = employeeName, Date = date }, cancellationToken: ct));
        }
    }
}
