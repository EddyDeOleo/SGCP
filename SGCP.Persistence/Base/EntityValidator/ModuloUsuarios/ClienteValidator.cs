
using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Persistence.Base.EntityValidator.ModuloUsuarios
{
    public class ClienteValidator : EntityValidator<Cliente>
    {
        private readonly ILogger<ClienteValidator> _logger;

        public ClienteValidator(ILogger<ClienteValidator> logger)
            : base(logger)
        {
            _logger = logger;
        }

        public override OperationResult ValidateForSave(Cliente entity)
        {
            if (entity == null)
                return OperationResult.FailureResult("El cliente no puede ser nulo");

            if (string.IsNullOrWhiteSpace(entity.Nombre))
                return OperationResult.FailureResult("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(entity.Apellido))
                return OperationResult.FailureResult("El apellido es obligatorio");

            if (string.IsNullOrWhiteSpace(entity.Username))
                return OperationResult.FailureResult("El username es obligatorio");

            if (string.IsNullOrWhiteSpace(entity.Password))
                return OperationResult.FailureResult("La contraseña es obligatoria");

            return OperationResult.SuccessResult("Cliente válido para guardar");
        }

        public override OperationResult ValidateForUpdate(Cliente entity)
        {
            var val = ValidateForSave(entity);
            if (!val.Success) return val;

            if (entity.IdUsuario <= 0)
                return OperationResult.FailureResult("El Id del cliente debe ser válido para actualizar");

            return OperationResult.SuccessResult("Cliente válido para actualizar");
        }

        public override OperationResult ValidateForRemove(Cliente entity)
        {
            if (entity == null)
                return OperationResult.FailureResult("El cliente no puede ser nulo");

            if (entity.IdUsuario <= 0)
                return OperationResult.FailureResult("El Id del cliente debe ser válido para eliminar");

            return OperationResult.SuccessResult("Cliente válido para eliminar");
        }
    }
}
