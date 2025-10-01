using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Persistence.Repositories.Base;

namespace SGCP.Persistence.Repositories.ModuloUsuarios
{
    public class ClienteRepositoryEF : BaseRepositoryEF<Cliente>, ICliente
    {
        public ClienteRepositoryEF(SGCPDbContext context, ILogger<ClienteRepositoryEF> logger)
            : base(context, logger) { }

        public async Task<Cliente> GetByUsername(string username)
        {
            _logger.LogInformation("Buscando cliente por username {Username}", username);
            return await _dbSet.FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task<List<Pedido>> GetPedidosByClienteId(int clienteId)
        {
            _logger.LogInformation("Obteniendo pedidos del cliente {ClienteId}", clienteId);
            var cliente = await _dbSet.Include(c => c.HistorialPedidos)
                                      .FirstOrDefaultAsync(c => c.IdUsuario == clienteId);
            return cliente?.HistorialPedidos ?? new List<Pedido>();
        }
    }
}
