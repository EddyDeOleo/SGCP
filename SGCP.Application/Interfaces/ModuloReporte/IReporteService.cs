using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloReporte.Reporte;

namespace SGCP.Application.Interfaces.ModuloReporte
{
    public interface IReporteService
    {
        Task<ServiceResult> GetReporte();

        Task<ServiceResult> GetReporteById(int id);

        Task<ServiceResult> CreateReporte(CreateReporteDTO createReporteDto);

        Task<ServiceResult> UpdateReporte(UpdateReporteDTO updateReporteDto);

        Task<ServiceResult> RemoveReporte(DeleteReporteDTO deleteReporteDto);
    }
}