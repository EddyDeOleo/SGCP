using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;

namespace SGCP.Persistence.Base
{
    public static class RepositoryLoggerHelper
    {
        public static void LogStart<TEntity>(ILogger logger, string action, object? context = null)
        {
            logger.LogInformation("{Entity}: iniciando acción {Action}. Contexto: {Context}",
                typeof(TEntity).Name, action, context ?? "N/A");
        }

        public static void LogSuccess<TEntity>(ILogger logger, string action, object? context = null)
        {
            logger.LogInformation("{Entity}: acción {Action} completada exitosamente. Contexto: {Context}",
                typeof(TEntity).Name, action, context ?? "N/A");
        }

        public static void LogWarning<TEntity>(ILogger logger, string message)
        {
            logger.LogWarning("{Entity}: {Message}", typeof(TEntity).Name, message);
        }

        public static void LogError<TEntity>(ILogger logger, Exception ex, string action, object? context = null)
        {
            logger.LogError(ex, "{Entity}: error durante la acción {Action}. Contexto: {Context}",
                typeof(TEntity).Name, action, context ?? "N/A");
        }

       public static async Task<OperationResult> ExecuteLoggedAsync<TEntity>(
       ILogger logger,
       string action,
       Func<Task<OperationResult>> operation,
       object? context = null)
        {
            LogStart<TEntity>(logger, action, context);

            try
            {
                var result = await operation();

                if (result.Success)
                    LogSuccess<TEntity>(logger, action, context);
                else
                    LogWarning<TEntity>(logger, result.Message);

                return result;
            }
            catch (Exception ex)
            {
                LogError<TEntity>(logger, ex, action, context);
                return OperationResult.FailureResult(ex.Message);
            }
        }
    }
}
