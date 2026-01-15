using Npgsql;
using System.Data;

namespace TimeTracking.API.Repositories
{
    public abstract class BaseRepository
    {
        private readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("Connection string is missing");
        }

        protected IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
