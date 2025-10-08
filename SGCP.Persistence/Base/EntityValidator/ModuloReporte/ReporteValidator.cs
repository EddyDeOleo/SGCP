using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeReporte;


namespace SGCP.Persistence.Base.EntityValidator.ModuloReporte
{
    public class ReporteValidator : EntityValidator<Reporte>
    {
        public ReporteValidator(ILogger<ReporteValidator> logger) : base(logger) { }

        public override OperationResult ValidateForSave(Reporte entity)
        {
            var result = ValidateBase(entity);
            if (!result.Success)
                return result;

            if (entity.TotalVentas < 0 || entity.TotalPedidos < 0)
            {
                _logger.LogWarning("Valores inválidos de ventas o pedidos para el reporte");
                return OperationResult.FailureResult("El total de ventas o pedidos no puede ser negativo.");
            }

            return OperationResult.SuccessResult("Validación de guardado de reporte exitosa.");
        }

        public override OperationResult ValidateForUpdate(Reporte entity)
        {
            var validation = ValidateForSave(entity);
            if (!validation.Success)
                return validation;

            return OperationResult.SuccessResult("Validación de actualización de reporte exitosa.");
        }

        public override OperationResult ValidateForRemove(Reporte entity)
        {
            if (entity == null)
            {
                _logger.LogWarning("Reporte nulo no puede ser eliminado");
                return OperationResult.FailureResult("El reporte no puede ser nulo.");
            }

            return OperationResult.SuccessResult("Validación de eliminación de reporte exitosa.");
        }
    }
}
