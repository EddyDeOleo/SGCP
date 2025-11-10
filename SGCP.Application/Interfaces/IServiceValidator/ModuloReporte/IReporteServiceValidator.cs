

using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloReporte.Reporte;

namespace SGCP.Application.Interfaces.IServiceValidator.ModuloReporte
{
    public interface IReporteServiceValidator : IServiceValidatorBase<CreateReporteDTO, UpdateReporteDTO, DeleteReporteDTO>
    {

        Task<ServiceResult> ValidateReporteExistente(int idReporte);
        Task<ServiceResult> ValidateAdministradorExistente(int adminId);
    }
}
