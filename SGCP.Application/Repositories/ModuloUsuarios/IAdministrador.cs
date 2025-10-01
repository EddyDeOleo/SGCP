using SGCP.Domain.Entities.ModuloDeReporte;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Domain.Repository;

namespace SGCP.Application.Repositories.ModuloUsuarios
{
    public interface IAdministrador : IBaseRepository<Administrador>  
    {
        Task<Administrador> GetByUsername(string username);

        Task<List<Reporte>> GetReportesPorAdministrador(int adminId);

    }
}
