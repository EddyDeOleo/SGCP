using Microsoft.Extensions.Logging;

namespace SGCP.Application.Base;
public abstract class BaseService<T> where T : class
{
    private readonly ILogger<T> _logger;

    protected BaseService(ILogger<T> logger)
    {
        _logger = logger;
    }

    
    protected async Task<ServiceResult> ExecuteSafeAsync(
    string actionDescription,
    Func<Task<ServiceResult>> operation)
    {
        _logger.LogInformation($"Iniciando: {actionDescription}");
        var result = new ServiceResult();

        try
        {
            result = await operation();
            if (result.Success)
                _logger.LogInformation($"Finalizado correctamente: {actionDescription}");
            else
                _logger.LogWarning($"Finalizado con error: {actionDescription} - {result.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error en operación: {actionDescription}");
            result.Success = false;
            result.Message = $"Ocurrió un error al {actionDescription}";
        }

        return result;
    }
 
}


