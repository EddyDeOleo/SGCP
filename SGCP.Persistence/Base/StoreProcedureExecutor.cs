using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;


namespace SGCP.Persistence.Base
{
    public class StoredProcedureExecutor : IStoredProcedureExecutor
    {
        private readonly string _connectionString;
        private readonly ILogger<StoredProcedureExecutor> _logger;

        public StoredProcedureExecutor(IConfiguration configuration, ILogger<StoredProcedureExecutor> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<int> ExecuteAsync(string procedureName, Dictionary<string, object> parameters, SqlParameter? outputParam = null)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                foreach (var param in parameters)
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);

                if (outputParam != null)
                {
                    outputParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(outputParam);
                }

                await connection.OpenAsync();
                var result = await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Stored procedure {Proc} executed with result {Result}", procedureName, result);
                _logger.LogInformation("Parameters: {@Params}", parameters);

                return outputParam != null ? (int)outputParam.Value : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure {Proc}", procedureName);
                throw;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string procedureName, Func<SqlDataReader, T> map, Dictionary<string, object>? parameters = null)
        {
            var results = new List<T>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (parameters != null)
                    foreach (var param in parameters)
                        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (!reader.HasRows)
                {
                    _logger.LogInformation("Stored procedure {Proc} returned no rows.", procedureName);
                    return results;
                }

                while (await reader.ReadAsync())
                {
                    results.Add(map(reader));
                }

                _logger.LogInformation("Stored procedure {Proc} executed successfully with {Count} rows.", procedureName, results.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure {Proc}", procedureName);
                throw;
            }

            return results;
        }
    }
}
