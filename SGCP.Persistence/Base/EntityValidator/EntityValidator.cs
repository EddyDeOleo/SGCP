using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;


namespace SGCP.Persistence.Base.EntityValidator
{

    public abstract class EntityValidator<T> : IEntityValidator<T>
    {
        protected readonly ILogger _logger;

        protected EntityValidator(ILogger logger)
        {
            _logger = logger;
        }

        protected virtual OperationResult ValidateBase(T entity)
        {
            if (entity == null)
            {
                _logger.LogWarning("Entidad nula no puede ser procesada ({EntityType})", typeof(T).Name);
                return OperationResult.FailureResult($"La entidad {typeof(T).Name} no puede ser nula.");
            }

            return OperationResult.SuccessResult("Validación base exitosa.");
        }

        public virtual OperationResult ValidateForSave(T entity) => ValidateBase(entity);
        public virtual OperationResult ValidateForUpdate(T entity) => ValidateBase(entity);
        public virtual OperationResult ValidateForRemove(T entity) => ValidateBase(entity);
    }
}
