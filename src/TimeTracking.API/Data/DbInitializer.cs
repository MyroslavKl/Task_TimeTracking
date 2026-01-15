using Dapper;
using System.Data.Common;
using TimeTracking.API.Models;
using TimeTracking.API.Repositories;

namespace TimeTracking.API.Data
{
    public class DbInitializer : BaseRepository
    {
        private const string QUERY_CREATE_TABLE = @"
            create table if not exists TimeEntries (
                Id serial primary key,
                EmployeeName varchar(30) not null,
                Date timestamp not null,
                HoursWorked decimal(4,2) not null,
                Description varchar(100)
            );
        ";

        private const string QUERY_SELECT_COUNT = @"
            select count(*) 
            from TimeEntries
        ";

        private const string QUERY_SEED_DATA = @"
            insert into TimeEntries 
                 (EmployeeName, Date, HoursWorked, Description)
            VALUES 
                 (@EmployeeName, @Date, @HoursWorked, @Description)
        ";

        private List<TimeEntry> seedData = new List<TimeEntry>
        {
            new TimeEntry { EmployeeName = "Dev User", Date = DateTime.UtcNow, HoursWorked = 8, Description = "Initial Seed" },
            new TimeEntry { EmployeeName = "Test User", Date = DateTime.UtcNow, HoursWorked = 4.5m, Description = "Meeting" }
        };

        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(IConfiguration configuration, ILogger<DbInitializer> _logger)
            : base(configuration)
        {
            this._logger = _logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                using var connection = CreateConnection();

                if (connection is DbConnection dbConnection)
                {
                    await dbConnection.OpenAsync();
                }
                else
                {
                    connection.Open(); 
                }

                await connection.ExecuteAsync(QUERY_CREATE_TABLE);

                await SeedDataAsync(connection);

                _logger.LogInformation("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing database.");
                throw;
            }
        }

        private async Task SeedDataAsync(System.Data.IDbConnection connection)
        {
            var count = await connection.ExecuteScalarAsync<int>(QUERY_SELECT_COUNT);

            if (count == 0)
            {
                _logger.LogInformation("Seeding initial data...");

                await connection.ExecuteAsync(QUERY_SEED_DATA, seedData);
            }
            else 
            {
                _logger.LogInformation("Data is seeded with {count} records", count);
            }
        }
    }
}
