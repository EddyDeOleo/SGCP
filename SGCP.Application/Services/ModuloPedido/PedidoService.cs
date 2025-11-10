using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Base.ServiceValidator.ModuloPedido;
using SGCP.Application.Dtos.ModuloCarrito.CarritoProducto;
using SGCP.Application.Dtos.ModuloPedido.Pedido;
using SGCP.Application.Interfaces;
using SGCP.Application.Mappers;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloPedido;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeProducto;


namespace SGCP.Application.Services.ModuloPedido
{
    public sealed class PedidoService : BaseService<PedidoService>, IPedidoService
    {
        private readonly IPedido _pedidoRepository;
        private readonly ICarrito _carritoRepository;
        private readonly ICarritoProducto _carritoProductoRepo;
        private readonly IPedidoProducto _pedidoProductoRepo;
        private readonly ICliente _clienteRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProducto _productoRepository;
        private readonly PedidoServiceValidator _pedidoServiceValidator;

        public PedidoService(
            IPedido pedidoRepository,
            ICarrito carritoRepository,
            ICarritoProducto carritoProductoRepo,
            IPedidoProducto pedidoProductoRepo,
            ICliente clienteRepository,
            ICurrentUserService currentUserService,
            ILogger<PedidoService> logger,
            IProducto productoRepository,
            PedidoServiceValidator pedidoServiceValidator
        ) : base(logger)
        {
            _pedidoRepository = pedidoRepository;
            _carritoRepository = carritoRepository;
            _carritoProductoRepo = carritoProductoRepo;
            _pedidoProductoRepo = pedidoProductoRepo;
            _clienteRepository = clienteRepository;
            _currentUserService = currentUserService;
            _productoRepository = productoRepository;
            _pedidoServiceValidator = pedidoServiceValidator;
        }

     
        public async Task<ServiceResult> CreatePedido(CreatePedidoDTO dto)
        {
            return await ExecuteSafeAsync("crear pedido", async () =>
            {
                var dtoValidation = _pedidoServiceValidator.ValidateForCreate(dto);
                if (!dtoValidation.Success) return dtoValidation;

                var clienteValidation = await _pedidoServiceValidator.ValidateCliente(dto.ClienteId);
                if (!clienteValidation.Success) return clienteValidation;

                var carritoValidation = await _pedidoServiceValidator.ValidateCarrito(dto.CarritoId);
                if (!carritoValidation.Success) return carritoValidation;

                var productosValidation = await _pedidoServiceValidator.ValidateProductosCarrito(dto.CarritoId);
                if (!productosValidation.Success) return productosValidation;

                var productosCarrito = (List<CarritoProductoGetDTO>)productosValidation.Data;
                decimal total = productosCarrito.Sum(p => p.Precio * p.Cantidad);

                var pedido = PedidoMapper.ToEntity(dto);
                pedido.Total = total;
                pedido.Estado = "Pendiente";
                pedido.UsuarioModificacion = _currentUserService.GetUserId();

                var saveResult = await _pedidoRepository.Save(pedido);
                if (!saveResult.Success)
                    return new ServiceResult(false, saveResult.Message);

                foreach (var item in productosCarrito)
                    await _pedidoProductoRepo.AgregarProducto(pedido.IdPedido, item.ProductoId, item.Cantidad);

                return new ServiceResult(true, "Pedido creado exitosamente", PedidoMapper.ToDto(pedido));
            });
        }

     
        public async Task<ServiceResult> GetPedido()
        {
            return await ExecuteSafeAsync("obtener pedidos", async () =>
            {
                var opResult = await _pedidoRepository.GetAll();
                if (!opResult.Success || opResult.Data == null)
                    return new ServiceResult(false, "No se pudieron obtener los pedidos");

                var pedidosDto = ((List<Pedido>)opResult.Data)
                    .Select(PedidoMapper.ToDto)
                    .ToList();

                return new ServiceResult(true, "Pedidos obtenidos correctamente", pedidosDto);
            });
        }

     
        public async Task<ServiceResult> GetPedidoById(int id)
        {
            return await ExecuteSafeAsync($"obtener pedido con ID {id}", async () =>
            {
                var pedidoValidation = await _pedidoServiceValidator.ValidatePedidoExistente(id);
                if (!pedidoValidation.Success) return pedidoValidation;

                var pedido = (Pedido)pedidoValidation.Data;
                return new ServiceResult(true, "Pedido obtenido correctamente", PedidoMapper.ToDto(pedido));
            });
        }

       
        public async Task<ServiceResult> UpdatePedido(UpdatePedidoDTO dto)
        {
            return await ExecuteSafeAsync($"actualizar pedido con ID {dto.IdPedido}", async () =>
            {
                var pedidoValidation = await _pedidoServiceValidator.ValidatePedidoExistente(dto.IdPedido);
                if (!pedidoValidation.Success) return pedidoValidation;

                var pedido = (Pedido)pedidoValidation.Data;
                PedidoMapper.MapToEntity(pedido, dto);
                pedido.UsuarioModificacion = _currentUserService.GetUserId();

                var updateResult = await _pedidoRepository.Update(pedido);
                if (!updateResult.Success)
                    return new ServiceResult(false, updateResult.Message);

                return new ServiceResult(true, "Pedido actualizado correctamente", PedidoMapper.ToDto(pedido));
            });
        }

      
        public async Task<ServiceResult> RemovePedido(DeletePedidoDTO dto)
        {
            return await ExecuteSafeAsync($"eliminar pedido con ID {dto.IdPedido}", async () =>
            {
                var pedidoValidation = await _pedidoServiceValidator.ValidatePedidoExistente(dto.IdPedido);
                if (!pedidoValidation.Success) return pedidoValidation;

                var pedido = (Pedido)pedidoValidation.Data;
                var removeResult = await _pedidoRepository.Remove(pedido);
                if (!removeResult.Success)
                    return new ServiceResult(false, removeResult.Message);

                return new ServiceResult(true, "Pedido eliminado correctamente");
            });
        }

       
        public async Task<ServiceResult> FinalizarPedido(int idPedido)
        {
            return await ExecuteSafeAsync($"finalizar pedido con ID {idPedido}", async () =>
            {
                var pedidoValidation = await _pedidoServiceValidator.ValidatePedidoExistente(idPedido);
                if (!pedidoValidation.Success) return pedidoValidation;

                var pedido = (Pedido)pedidoValidation.Data;

                var estadoValidation = _pedidoServiceValidator.ValidateEstadoParaFinalizar(pedido);
                if (!estadoValidation.Success) return estadoValidation;

                var productosValidation = await _pedidoServiceValidator.ValidateProductosCarrito(pedido.CarritoId!.Value);
                if (!productosValidation.Success) return productosValidation;

                var carritoProductos = (List<CarritoProductoGetDTO>)productosValidation.Data;

                // Actualizar stock
                foreach (var item in carritoProductos)
                {
                    var productoResult = await _productoRepository.GetEntityBy(item.ProductoId);
                    if (!productoResult.Success || productoResult.Data == null)
                        return new ServiceResult(false, $"No se encontró el producto con ID {item.ProductoId}.");

                    var producto = (Producto)productoResult.Data;
                    if (producto.Stock < item.Cantidad)
                        return new ServiceResult(false, $"No hay suficiente stock del producto {producto.Nombre}.");

                    producto.Stock -= item.Cantidad;
                    var updateProducto = await _productoRepository.Update(producto);
                    if (!updateProducto.Success)
                        return new ServiceResult(false, $"Error al actualizar el stock del producto {producto.Nombre}.");
                }

                pedido.Estado = "Finalizado";
                var updatePedido = await _pedidoRepository.Update(pedido);

                return new ServiceResult(
                    updatePedido.Success,
                    updatePedido.Success ? "Pedido finalizado correctamente y stock actualizado." : updatePedido.Message,
                    updatePedido.Data
                );
            });
        }

    
        public async Task<ServiceResult> CancelarPedido(int idPedido)
        {
            return await ExecuteSafeAsync($"cancelar pedido con ID {idPedido}", async () =>
            {
                var pedidoValidation = await _pedidoServiceValidator.ValidatePedidoExistente(idPedido);
                if (!pedidoValidation.Success) return pedidoValidation;

                var pedido = (Pedido)pedidoValidation.Data;

                var estadoValidation = _pedidoServiceValidator.ValidateEstadoParaCancelar(pedido);
                if (!estadoValidation.Success) return estadoValidation;

                pedido.Estado = "Cancelado";
                var updateResult = await _pedidoRepository.Update(pedido);

                return new ServiceResult(
                    updateResult.Success,
                    updateResult.Success ? "Pedido cancelado correctamente." : updateResult.Message,
                    updateResult.Data
                );
            });
        }
    }
}



