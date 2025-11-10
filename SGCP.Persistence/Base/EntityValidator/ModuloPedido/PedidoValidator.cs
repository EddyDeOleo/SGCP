using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDePedido;

namespace SGCP.Persistence.Base.EntityValidator.ModuloPedido
{

    public class PedidoValidator : EntityValidator<Pedido>
    {
        public PedidoValidator(ILogger<PedidoValidator> logger) : base(logger) { }

        public override OperationResult ValidateForSave(Pedido entity)
        {
            var baseResult = ValidateBase(entity);
            if (!baseResult.Success)
                return baseResult;

            if (entity.ClienteId <= 0)
                return OperationResult.FailureResult("El cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(entity.Estado) || entity.Estado.Length > 30)
                return OperationResult.FailureResult("El estado del pedido es obligatorio y no puede exceder 30 caracteres.");

            if (entity.Total < 0)
                return OperationResult.FailureResult("El total del pedido no puede ser negativo.");

            return OperationResult.SuccessResult("Validación de guardado exitosa.");
        }

        public override OperationResult ValidateForUpdate(Pedido entity)
        {
            var result = ValidateForSave(entity);
            if (!result.Success)
                return result;

            if (entity.IdPedido <= 0)
                return OperationResult.FailureResult("El Id del pedido debe ser válido para actualizar.");

            return OperationResult.SuccessResult("Validación de actualización exitosa.");
        }

        public override OperationResult ValidateForRemove(Pedido entity)
        {
            var result = ValidateBase(entity);
            if (!result.Success)
                return result;

            if (entity.IdPedido <= 0)
                return OperationResult.FailureResult("El Id del pedido debe ser válido para eliminar.");

            return OperationResult.SuccessResult("Validación de eliminación exitosa.");
        }
    }

}
