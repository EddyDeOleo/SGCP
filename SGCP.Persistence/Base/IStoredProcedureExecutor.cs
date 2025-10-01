
using Microsoft.Data.SqlClient;


namespace SGCP.Persistence.Base

{
    public interface IStoredProcedureExecutor
    {
 
        Task<int> ExecuteAsync(
            string procedureName,
            Dictionary<string, object> parameters,
            SqlParameter? outputParam = null);

    
        Task<List<T>> QueryAsync<T>(
            string procedureName,
            Func<SqlDataReader, T> map,
            Dictionary<string, object>? parameters = null);
    }
}
