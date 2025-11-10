

using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;

namespace SGCP.Application.Base.ServiceValidator
{
    public abstract class ServiceValidator<T> where T : class
    {
        protected readonly ILogger<T> _logger;

        protected ServiceValidator(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected static ServiceResult ToServiceResult(OperationResult op)
            => new ServiceResult(op.Success, op.Message, op.Data);

        protected ServiceResult Success(string message = "Operación exitosa.", dynamic? data = null)
            => new ServiceResult(true, message, data);

        protected ServiceResult Failure(string message)
            => new ServiceResult(false, message);

        protected ServiceResult ValidateNotNull(object? obj, string name)
            => obj == null
                ? Failure($"{name} no puede ser nulo.")
                : Success($"{name} válido.");

        protected ServiceResult ValidateId(int id, string name)
            => id <= 0
                ? Failure($"{name} debe ser mayor que cero.")
                : Success($"{name} válido.");
    }
}


