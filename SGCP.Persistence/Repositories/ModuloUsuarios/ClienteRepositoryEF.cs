using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Base;
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
            _logger.LogInformation("Obteniendo pedidos para cliente con ID {ClienteId}", clienteId);

            if (clienteId <= 0)
            {
                _logger.LogWarning("ID de cliente inválido: {ClienteId}", clienteId);
                return new List<Pedido>();
            }

            try
            {
                var cliente = await _context.Cliente.FindAsync(clienteId);
                if (cliente == null)
                {
                    _logger.LogWarning("No se encontró cliente con ID {ClienteId}", clienteId);
                    return new List<Pedido>();
                }

                var pedidos = await _context.Pedido
                    .Where(p => p.ClienteId == clienteId)
                    .ToListAsync();

                return pedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos del cliente {ClienteId}", clienteId);
                return new List<Pedido>();
            }
        }

        public override async Task<OperationResult> GetAll()
        {
            _logger.LogInformation("Obteniendo clientes en orden descendente");
            try
            {
                var list = await _dbSet
                    .OrderByDescending(a => a.FechaCreacion)
                    .ToListAsync();

                return OperationResult.SuccessResult("Clientes obtenidos correctamente", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
                return OperationResult.FailureResult($"Error al obtener clientes: {ex.Message}");
            }
        }
    }
}
