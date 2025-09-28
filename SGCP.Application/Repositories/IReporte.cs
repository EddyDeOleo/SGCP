using SGCP.Domain.Repository;   
using SGCP.Domain.Entities.ModuloDeReporte; 

namespace SGCP.Application.Repositories
{
    internal interface IReporte : IBaseRepository<Reporte>
    {
        Task<List<Reporte>> GetByFecha(DateTime fechaInicio, DateTime fechaFin);

        Task<List<Reporte>> GetByAdministradorId(int adminId);
    }
}
