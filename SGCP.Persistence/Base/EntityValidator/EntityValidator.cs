using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;


namespace SGCP.Persistence.Base.EntityValidator
{

    public abstract class EntityValidator<TEntity> where TEntity : class
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

        protected OperationResult ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogWarning("Campo requerido vacío: {FieldName}", fieldName);
                return OperationResult.FailureResult($"El campo {fieldName} es requerido.");
            }
            return OperationResult.SuccessResult("Campo válido");
        }

        protected OperationResult ValidateId(int id, string fieldName = "Id")
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido: {FieldName} = {Id}", fieldName, id);
                return OperationResult.FailureResult($"El {fieldName} debe ser mayor a 0.");
            }
            return OperationResult.SuccessResult("ID válido");
        }

        protected OperationResult ValidateLength(string value, string fieldName, int maxLength, int minLength = 0)
        {
            if (value == null) return OperationResult.SuccessResult("Campo válido");

            if (value.Length < minLength)
            {
                _logger.LogWarning("{FieldName} muy corto: {Length} (mínimo: {MinLength})", fieldName, value.Length, minLength);
                return OperationResult.FailureResult($"{fieldName} debe tener al menos {minLength} caracteres.");
            }

            if (value.Length > maxLength)
            {
                _logger.LogWarning("{FieldName} muy largo: {Length} (máximo: {MaxLength})", fieldName, value.Length, maxLength);
                return OperationResult.FailureResult($"{fieldName} no puede exceder {maxLength} caracteres.");
            }

            return OperationResult.SuccessResult("Longitud válida");
        }
    }
}
