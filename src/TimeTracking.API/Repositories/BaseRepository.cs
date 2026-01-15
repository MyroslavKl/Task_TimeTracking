using Dapper;
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

        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(new CommandDefinition(sql, param, cancellationToken: ct));
        }

        protected async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(sql, param, cancellationToken: ct));
        }

        protected async Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(new CommandDefinition(sql, param, cancellationToken: ct));
        }

        protected async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken ct = default)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<T>(new CommandDefinition(sql, param, cancellationToken: ct));
        }
    }
}
