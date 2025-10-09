using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloPedido.Pedido;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloPedido;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeUsuarios;


namespace SGCP.Application.Services
{
    public sealed class PedidoService : IPedidoService
    {
        private readonly IPedido _pedidoRepository;
        private readonly ICarrito _carritoRepository;
        private readonly ILogger<PedidoService> _logger;
        private readonly ISessionService _sessionService; 

        public PedidoService(
            IPedido pedidoRepository,
            ICarrito carritoRepository,
            ILogger<PedidoService> logger,
            ISessionService sessionService)
        {
            _pedidoRepository = pedidoRepository;
            _carritoRepository = carritoRepository;
            _logger = logger;
            _sessionService = sessionService;
        }

        public async Task<ServiceResult> CreatePedido(CreatePedidoDTO createPedidoDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation("Creando nuevo pedido");

            try
            {
                // CU-09: Validar cliente logueado
                if (!_sessionService.ClienteIdLogueado.HasValue)
                {
                    result.Success = false;
                    result.Message = "Debe iniciar sesión para realizar un pedido";
                    return result;
                }

                int clienteId = _sessionService.ClienteIdLogueado.Value;

                // CU-09: Validar que el carrito tiene productos
                var carritoResult = await _carritoRepository.GetEntityBy(createPedidoDto.CarritoId ?? 0);
                if (!carritoResult.Success || carritoResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "El carrito no existe";
                    return result;
                }

                var carrito = (Carrito)carritoResult.Data;
                if (carrito.Productos.Count == 0)
                {
                    result.Success = false;
                    result.Message = "El carrito está vacío, agregue productos antes de realizar un pedido";
                    return result;
                }

                var pedido = new Pedido(carrito, new Cliente(clienteId, "", "", "", ""))
                {
                    Total = carrito.CalcularTotal(),
                    FechaCreacion = createPedidoDto.FechaCreacion,
                };

                pedido.ActualizarEstado(createPedidoDto.Estado);

                var opResult = await _pedidoRepository.Save(pedido);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = "No se pudo crear el pedido";
                    _logger.LogWarning("Error al crear pedido");
                    return result;
                }

                result.Success = true;
                result.Message = "Pedido creado exitosamente";
                result.Data = new PedidoGetDTO
                {
                    IdPedido = pedido.IdPedido,
                    ClienteId = pedido.Cliente.IdUsuario,
                    CarritoId = pedido.Carrito.IdCarrito,
                    Total = pedido.Total,
                    Estado = pedido.Estado,
                    FechaCreacion = pedido.FechaCreacion
                };

                _logger.LogInformation("Pedido creado correctamente");
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
                if (!_sessionService.ClienteIdLogueado.HasValue)
                {
                    result.Success = false;
                    result.Message = "Debe iniciar sesión para consultar pedidos";
                    return result;
                }

                var opResult = await _pedidoRepository.GetAll();
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "No se pudieron obtener los pedidos";
                    _logger.LogWarning("Error al obtener pedidos");
                    return result;
                }

                int clienteId = _sessionService.ClienteIdLogueado.Value;

                var pedidos = ((List<Pedido>)opResult.Data)
                    .Where(p => p.Cliente.IdUsuario == clienteId) 
                    .Select(p => new PedidoGetDTO
                    {
                        IdPedido = p.IdPedido,
                        ClienteId = p.Cliente.IdUsuario,
                        CarritoId = p.Carrito.IdCarrito,
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
                if (!_sessionService.ClienteIdLogueado.HasValue)
                {
                    result.Success = false;
                    result.Message = "Debe iniciar sesión para consultar pedidos";
                    return result;
                }

                var opResult = await _pedidoRepository.GetEntityBy(id);
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Pedido no encontrado";
                    _logger.LogWarning($"Pedido con ID {id} no encontrado");
                    return result;
                }

                var p = (Pedido)opResult.Data;

                if (p.Cliente.IdUsuario != _sessionService.ClienteIdLogueado.Value)
                {
                    result.Success = false;
                    result.Message = "No tiene permiso para ver este pedido";
                    return result;
                }

                var dto = new PedidoGetDTO
                {
                    IdPedido = p.IdPedido,
                    ClienteId = p.Cliente.IdUsuario,
                    CarritoId = p.Carrito.IdCarrito,
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
                if (!_sessionService.ClienteIdLogueado.HasValue)
                {
                    result.Success = false;
                    result.Message = "Debe iniciar sesión para actualizar pedidos";
                    return result;
                }

                var opResult = await _pedidoRepository.GetEntityBy(updatePedidoDto.IdPedido);
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Pedido no encontrado";
                    return result;
                }

                var existingPedido = (Pedido)opResult.Data;
                if (existingPedido.Cliente.IdUsuario != _sessionService.ClienteIdLogueado.Value)
                {
                    result.Success = false;
                    result.Message = "No tiene permiso para actualizar este pedido";
                    return result;
                }

                existingPedido.Total = updatePedidoDto.Total;
                existingPedido.FechaCreacion = updatePedidoDto.FechaCreacion;
                existingPedido.ActualizarEstado(updatePedidoDto.Estado);

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
                if (!_sessionService.ClienteIdLogueado.HasValue)
                {
                    result.Success = false;
                    result.Message = "Debe iniciar sesión para eliminar pedidos";
                    return result;
                }

                var opResult = await _pedidoRepository.GetEntityBy(deletePedidoDto.IdPedido);
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Pedido no encontrado";
                    return result;
                }

                var existingPedido = (Pedido)opResult.Data;
                if (existingPedido.Cliente.IdUsuario != _sessionService.ClienteIdLogueado.Value)
                {
                    result.Success = false;
                    result.Message = "No tiene permiso para eliminar este pedido";
                    return result;
                }

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

