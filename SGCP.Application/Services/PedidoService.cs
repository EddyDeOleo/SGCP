using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloCarrito.CarritoProducto;
using SGCP.Application.Dtos.ModuloPedido.Pedido;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloPedido;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeUsuarios;


namespace SGCP.Application.Services
{
    public sealed class PedidoService : IPedidoService
    {
        private readonly IPedido _pedidoRepository;
        private readonly ICarrito _carritoRepository;
        private readonly ICarritoProducto _carritoProductoRepo;
        private readonly IPedidoProducto _pedidoProductoRepo;
        private readonly ICliente _clienteRepository;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(
            IPedido pedidoRepository,
            ICarrito carritoRepository,
            ICarritoProducto carritoProductoRepo,
            IPedidoProducto pedidoProductoRepo,
            ICliente clienteRepository,
            ILogger<PedidoService> logger
           )
        {
            _pedidoRepository = pedidoRepository;
            _carritoRepository = carritoRepository;
            _carritoProductoRepo = carritoProductoRepo;
            _pedidoProductoRepo = pedidoProductoRepo;
            _clienteRepository = clienteRepository;
            _logger = logger;
        }

        public async Task<ServiceResult> CreatePedido(CreatePedidoDTO createPedidoDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation("Creando nuevo pedido");

            try
            {
                // ✅ Validar que el cliente existe
                var clienteResult = await _clienteRepository.GetEntityBy(createPedidoDto.ClienteId);
                if (!clienteResult.Success)
                {
                    result.Success = false;
                    result.Message = "Cliente no encontrado";
                    return result;
                }

                // ✅ Validar que el carrito existe
                var carritoResult = await _carritoRepository.GetEntityBy(createPedidoDto.CarritoId);
                if (!carritoResult.Success)
                {
                    result.Success = false;
                    result.Message = "Carrito no encontrado";
                    return result;
                }

                // ✅ Obtener productos del carrito
                var productosResult = await _carritoProductoRepo.GetProductosByCarritoId(createPedidoDto.CarritoId);
                if (!productosResult.Success || productosResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Error al obtener productos del carrito";
                    return result;
                }

                var productosCarrito = (List<CarritoProductoGetDTO>)productosResult.Data;

                if (!productosCarrito.Any())
                {
                    result.Success = false;
                    result.Message = "El carrito está vacío";
                    return result;
                }

                decimal total = 0;
                foreach (var item in productosCarrito)
                {
                    total += item.Precio * item.Cantidad;
                }

                 var pedido = new Pedido
                {
                    ClienteId = createPedidoDto.ClienteId,
                    CarritoId = createPedidoDto.CarritoId,
                    FechaCreacion = DateTime.Now,
                    Estado = "Pendiente",
                    Total = total,

                    Cliente = new Cliente
                    {
                        IdUsuario = createPedidoDto.ClienteId
                    },

                    Carrito = new Carrito
                    {
                        IdCarrito = createPedidoDto.CarritoId
                    }
                };

                var savePedidoResult = await _pedidoRepository.Save(pedido);
                if (!savePedidoResult.Success)
                {
                    result.Success = false;
                    result.Message = "No se pudo crear el pedido";
                    _logger.LogWarning("Error al crear pedido");
                    return result;
                }

                foreach (var item in productosCarrito)
                {
                    await _pedidoProductoRepo.AgregarProducto(
                        pedido.IdPedido,
                        item.ProductoId,
                        item.Cantidad
                    );
                }

                result.Success = true;
                result.Message = "Pedido creado exitosamente";
                result.Data = new PedidoGetDTO
                {
                    IdPedido = pedido.IdPedido,
                    ClienteId = pedido.ClienteId,
                    CarritoId = pedido.CarritoId,
                    Total = pedido.Total,
                    Estado = pedido.Estado,
                    FechaCreacion = pedido.FechaCreacion
                };

                _logger.LogInformation($"Pedido {pedido.IdPedido} creado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el pedido");
                result.Success = false;
                result.Message = "Ocurrió un error al crear el pedido";
            }

            return result;
        }
        public async Task<ServiceResult> GetPedido()
        {
            var result = new ServiceResult();
            _logger.LogInformation("Obteniendo todos los pedidos");

            try
            {
                var opResult = await _pedidoRepository.GetAll();
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "No se pudieron obtener los pedidos";
                    _logger.LogWarning("Error al obtener pedidos");
                    return result;
                }

                var pedidos = ((List<Pedido>)opResult.Data)
                    .Select(p => new PedidoGetDTO
                    {
                        IdPedido = p.IdPedido,
                        ClienteId = p.ClienteId,
                        CarritoId = p.CarritoId,
                        Total = p.Total,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion
                    }).ToList();

                result.Success = true;
                result.Data = pedidos;
                result.Message = "Pedidos obtenidos correctamente";
                _logger.LogInformation("Pedidos obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los pedidos");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener los pedidos";
            }

            return result;
        }

        public async Task<ServiceResult> GetPedidoById(int id)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Obteniendo pedido con ID: {id}");

            try
            {
                var opResult = await _pedidoRepository.GetEntityBy(id);
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Pedido no encontrado";
                    _logger.LogWarning($"Pedido con ID {id} no encontrado");
                    return result;
                }

                var p = (Pedido)opResult.Data;

                var dto = new PedidoGetDTO
                {
                    IdPedido = p.IdPedido,
                    ClienteId = p.ClienteId,
                    CarritoId = p.CarritoId,
                    Total = p.Total,
                    Estado = p.Estado,
                    FechaCreacion = p.FechaCreacion
                };

                result.Success = true;
                result.Data = dto;
                result.Message = "Pedido obtenido correctamente";
                _logger.LogInformation($"Pedido con ID {id} obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el pedido con ID {id}");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener el pedido";
            }

            return result;
        }

        public async Task<ServiceResult> UpdatePedido(UpdatePedidoDTO updatePedidoDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Actualizando pedido con ID: {updatePedidoDto.IdPedido}");

            try
            {
                var opResult = await _pedidoRepository.GetEntityBy(updatePedidoDto.IdPedido);
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Pedido no encontrado";
                    return result;
                }

                var existingPedido = (Pedido)opResult.Data;

                existingPedido.Estado = updatePedidoDto.Estado;

                var updateResult = await _pedidoRepository.Update(existingPedido);
                if (!updateResult.Success)
                {
                    result.Success = false;
                    result.Message = "No se pudo actualizar el pedido";
                    return result;
                }

                result.Success = true;
                result.Message = "Pedido actualizado exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el pedido con ID {updatePedidoDto.IdPedido}");
                result.Success = false;
                result.Message = "Ocurrió un error al actualizar el pedido";
            }

            return result;
        }

        public async Task<ServiceResult> RemovePedido(DeletePedidoDTO deletePedidoDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Eliminando pedido con ID: {deletePedidoDto.IdPedido}");

            try
            {
                var opResult = await _pedidoRepository.GetEntityBy(deletePedidoDto.IdPedido);
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Pedido no encontrado";
                    return result;
                }

                var existingPedido = (Pedido)opResult.Data;

                var removeResult = await _pedidoRepository.Remove(existingPedido);
                if (!removeResult.Success)
                {
                    result.Success = false;
                    result.Message = "No se pudo eliminar el pedido";
                    return result;
                }

                result.Success = true;
                result.Message = "Pedido eliminado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el pedido con ID {deletePedidoDto.IdPedido}");
                result.Success = false;
                result.Message = "Ocurrió un error al eliminar el pedido";
            }

            return result;
        }
    }
}

