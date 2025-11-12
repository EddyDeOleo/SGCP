using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeReporte;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Persistence.Repositories.Base;


namespace SGCP.Persistence.Repositories.ModuloUsuarios
{
    public class AdministradorRepositoryEF : BaseRepositoryEF<Administrador>, IAdministrador
    {
        public AdministradorRepositoryEF(SGCPDbContext context, ILogger<AdministradorRepositoryEF> logger)
            : base(context, logger) { }

        public async Task<Administrador> GetByUsername(string username)
        {
            _logger.LogInformation("Buscando administrador por username {Username}", username);
            try
            {
                var admin = await _dbSet.FirstOrDefaultAsync(a => a.Username == username);
                if (admin == null)
                    _logger.LogWarning("No se encontró administrador con username {Username}", username);

                return admin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener administrador con username {Username}", username);
                throw;
            }
        }

        public async Task<List<Reporte>> GetReportesPorAdministrador(int adminId)
        {
            _logger.LogInformation("Obteniendo reportes del administrador {AdminId}", adminId);
            try
            {
                var reportes = await _context.Set<Reporte>()
                    .Where(r => r.AdminId == adminId)
                    .ToListAsync();

                if (!reportes.Any())
                {
                    _logger.LogWarning("No se encontraron reportes para el administrador {AdminId}", adminId);
                }

                return reportes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reportes del administrador {AdminId}", adminId);
                throw;
            }
        }

        public override async Task<OperationResult> GetAll()
        {
            _logger.LogInformation("Obteniendo administradores en orden descendente");
            try
            {
                var list = await _dbSet
                    .OrderByDescending(a => a.FechaCreacion)
                    .ToListAsync();

                return OperationResult.SuccessResult("Administradores obtenidos correctamente", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener administradores");
                return OperationResult.FailureResult($"Error al obtener administradores: {ex.Message}");
            }
        }
    }
}
