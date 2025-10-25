using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Domain.Base;
using SGCP.Persistence.Base;


namespace SGCP.Persistence.Repositories.ModuloCarrito
{
    public class CarritoProductoRepositoryAdo : ICarritoProducto
    {

        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<CarritoProductoRepositoryAdo> _logger;
        private readonly ICarrito _carritoRepository; // ✅ Inyectar repositorio de Carrito
        private readonly IProducto _productoRepository; // ✅ Inyectar repositorio de Producto

        public CarritoProductoRepositoryAdo(
            IStoredProcedureExecutor spExecutor,
            ILogger<CarritoProductoRepositoryAdo> logger,
            ICarrito carritoRepository,
            IProducto productoRepository)
        {
            _spExecutor = spExecutor;
            _logger = logger;
            _carritoRepository = carritoRepository;
            _productoRepository = productoRepository;
        }

        public async Task<OperationResult> AgregarProducto(int carritoId, int productoId, int cantidad)
        {
            try
            {
                // ✅ Validar usando el repositorio existente
                var carritoExists = await _carritoRepository.Exists(c => c.IdCarrito == carritoId);
                if (!carritoExists)
                {
                    return OperationResult.FailureResult("El carrito no existe");
                }

                var productoExists = await _productoRepository.Exists(p => p.IdProducto == productoId);
                if (!productoExists)
                {
                    return OperationResult.FailureResult("El producto no existe");
                }

                var parameters = new Dictionary<string, object>
            {
                { "@CarritoId", carritoId },
                { "@ProductoId", productoId },
                { "@Cantidad", cantidad }
            };

                await _spExecutor.ExecuteAsync("sp_AgregarProductoAlCarrito", parameters);
                return OperationResult.SuccessResult("Producto agregado al carrito");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto al carrito");
                return OperationResult.FailureResult("Error al agregar producto");
            }
        }
    }
}
