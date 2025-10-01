using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Persistence.Repositories.Base;
using System;
using System.Threading.Tasks;

namespace SGCP.Persistence.Repositories.ModuloUsuarios
{
    public class UsuarioRepositoryEF : BaseRepositoryEF<Usuario>, IUsuario
    {
        public UsuarioRepositoryEF(SGCPDbContext context, ILogger<UsuarioRepositoryEF> logger)
            : base(context, logger) { }

        public async Task<Usuario> GetByUsername(string username)
        {
            _logger.LogInformation("Buscando usuario por username {Username}", username);
            try
            {
                var usuario = await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
                if (usuario == null)
                    _logger.LogWarning("No se encontró usuario con username {Username}", username);

                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con username {Username}", username);
                throw;
            }
        }
    }
}
