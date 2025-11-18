using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Persistence.Base.IEntityValidator;


namespace SGCP.Persistence.Base.EntityValidator
{

    public abstract class EntityValidator<TEntity> : IEntityValidator<TEntity> where TEntity : class
    {
        protected readonly ILogger _logger;

        protected EntityValidator(ILogger logger)
        {
            _logger = logger;
        }

        public abstract OperationResult ValidateForSave(TEntity entity);
        public abstract OperationResult ValidateForUpdate(TEntity entity);
        public abstract OperationResult ValidateForRemove(TEntity entity);

        protected virtual OperationResult ValidateBase(TEntity entity)
        {
            if (entity == null)
            {
                _logger.LogWarning("Validación fallida: {EntityType} es nulo", typeof(TEntity).Name);
                return OperationResult.FailureResult($"El objeto {typeof(TEntity).Name} no puede ser nulo.");
            }

            _logger.LogDebug("Validación base exitosa para {EntityType}", typeof(TEntity).Name);
            return OperationResult.SuccessResult("Validación exitosa");
        }
    }
}
