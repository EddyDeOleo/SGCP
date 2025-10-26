
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloPedido;
using SGCP.Domain.Base;
using SGCP.Persistence.Base;

namespace SGCP.Persistence.Repositories.ModuloPedido
{
    public class PedidoProductoRepositoryAdo : IPedidoProducto
    {
        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<PedidoProductoRepositoryAdo> _logger;

        public PedidoProductoRepositoryAdo(
            IStoredProcedureExecutor spExecutor,
            ILogger<PedidoProductoRepositoryAdo> logger)
        {
            _spExecutor = spExecutor;
            _logger = logger;
        }

        public async Task<OperationResult> AgregarProducto(int pedidoId, int productoId, int cantidad)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@PedidoId", pedidoId },
                    { "@ProductoId", productoId },
                    { "@Cantidad", cantidad }
                };

                await _spExecutor.ExecuteAsync("sp_AgregarProductoAlPedido", parameters);
                return OperationResult.SuccessResult("Producto agregado al pedido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto al pedido");
                return OperationResult.FailureResult("Error al agregar producto");
            }
        }

    }
}
