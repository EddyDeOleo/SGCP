
using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Persistence.Base.EntityValidator.ModuloUsuarios
{
    public class AdministradorValidator : EntityValidator<Administrador>
    {
        private readonly ILogger<AdministradorValidator> _logger;

        public AdministradorValidator(ILogger<AdministradorValidator> logger)
            : base(logger)
        {
            _logger = logger;
        }

        public override OperationResult ValidateForSave(Administrador entity)
        {
            if (entity == null)
                return OperationResult.FailureResult("El administrador no puede ser nulo");

            if (string.IsNullOrWhiteSpace(entity.Nombre))
                return OperationResult.FailureResult("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(entity.Apellido))
                return OperationResult.FailureResult("El apellido es obligatorio");

            if (string.IsNullOrWhiteSpace(entity.Username))
                return OperationResult.FailureResult("El username es obligatorio");

            if (string.IsNullOrWhiteSpace(entity.Password))
                return OperationResult.FailureResult("La contraseña es obligatoria");

            return OperationResult.SuccessResult("Administrador válido para guardar");
        }

        public override OperationResult ValidateForUpdate(Administrador entity)
        {
            var val = ValidateForSave(entity);
            if (!val.Success) return val;

            if (entity.IdUsuario <= 0)
                return OperationResult.FailureResult("El Id del administrador debe ser válido para actualizar");

            return OperationResult.SuccessResult("Administrador válido para actualizar");
        }

        public override OperationResult ValidateForRemove(Administrador entity)
        {
            if (entity == null)
                return OperationResult.FailureResult("El administrador no puede ser nulo");

            if (entity.IdUsuario <= 0)
                return OperationResult.FailureResult("El Id del administrador debe ser válido para eliminar");

            return OperationResult.SuccessResult("Administrador válido para eliminar");
        }
    }
}
