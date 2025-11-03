

using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloCarrito.Carrito;
using SGCP.Application.Dtos.ModuloCarrito.CarritoProducto;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDeProducto;

namespace SGCP.Application.Services
    {
        public sealed class CarritoService : ICarritoService
        {
        private readonly ICarrito _carritoRepository;
        private readonly ICarritoProducto _carritoProductoRepo; 
        private readonly ILogger<CarritoService> _logger;
        private readonly IProducto _productoRepository;


        public CarritoService(
            ICarrito carritoRepository,
            ICarritoProducto carritoProductoRepo, 
            ILogger<CarritoService> logger,
            IProducto productoRepository
            )
        {
            _carritoRepository = carritoRepository;
            _carritoProductoRepo = carritoProductoRepo; 
            _logger = logger;
            _productoRepository = productoRepository;
        }


        public async Task<ServiceResult> AgregarProductoAlCarrito(int carritoId, AgregarProductoDTO dto)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Agregando producto {dto.ProductoId} al carrito {carritoId}");

            try
            {
                // Validar que el carrito existe
                var carritoResult = await _carritoRepository.GetEntityBy(carritoId);
                if (!carritoResult.Success)
                {
                    result.Success = false;
                    result.Message = "Carrito no encontrado";
                    return result;
                }

                // Validar que el producto existe y obtener stock
                var productoResult = await _productoRepository.GetEntityBy(dto.ProductoId);
                if (!productoResult.Success)
                {
                    result.Success = false;
                    result.Message = "Producto no encontrado";
                    return result;
                }

                var producto = (Producto)productoResult.Data;

                // Validar stock disponible
                if (producto.Stock < dto.Cantidad)
                {
                    result.Success = false;
                    result.Message = $"Stock insuficiente. Disponible: {producto.Stock}, Solicitado: {dto.Cantidad}";
                    return result;
                }

                if (dto.Cantidad <= 0)
                {
                    result.Success = false;
                    result.Message = "La cantidad debe ser mayor a cero";
                    return result;
                }

                var addResult = await _carritoProductoRepo.AgregarProducto(carritoId, dto.ProductoId, dto.Cantidad);

                result.Success = addResult.Success;
                result.Message = addResult.Message;

                _logger.LogInformation($"Producto agregado al carrito correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto al carrito");
                result.Success = false;
                result.Message = "Ocurrió un error al agregar el producto";
            }

            return result;
        }

        public async Task<ServiceResult> CreateCarrito(CreateCarritoDTO createCarritoDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation("Creando nuevo carrito");

            try
            {
                var carrito = new Carrito
                {
                    ClienteId = createCarritoDto.ClienteId
                   
                };

                var opResult = await _carritoRepository.Save(carrito);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = "No se pudo crear el carrito";
                    _logger.LogWarning("Error al crear carrito");
                    return result;
                }

                result.Success = true;
                result.Message = "Carrito creado exitosamente";
                result.Data = new CarritoGetDTO
                {
                    CarritoId = carrito.IdCarrito,
                    ClienteId = carrito.ClienteId,
                    FechaCreacion = carrito.FechaCreacion,
                    FechaModificacion = carrito.FechaModificacion,
                    UsuarioModificacion = carrito.UsuarioModificacion,
                    Estatus = carrito.Estatus
                };

                _logger.LogInformation("Carrito creado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el carrito");
                result.Success = false;
                result.Message = "Ocurrió un error al crear el carrito";
            }

            return result;
        }

        public async Task<ServiceResult> GetCarrito()
        {
            var result = new ServiceResult();
            _logger.LogInformation("Obteniendo todos los carritos");

            try
            {
                var opResult = await _carritoRepository.GetAll();

                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "No se pudieron obtener los carritos";
                    _logger.LogWarning("Error al obtener carritos");
                    return result;
                }

                var carritos = ((List<Carrito>)opResult.Data)
                    .Select(c => new CarritoGetDTO
                    {
                        CarritoId = c.IdCarrito,
                        ClienteId = c.ClienteId,
                        FechaCreacion = c.FechaCreacion,
                        FechaModificacion = c.FechaModificacion,
                        UsuarioModificacion = c.UsuarioModificacion,
                        Estatus = c.Estatus
                    }).ToList();

                result.Success = true;
                result.Data = carritos;
                result.Message = "Carritos obtenidos correctamente";
                _logger.LogInformation("Carritos obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los carritos");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener los carritos";
            }

            return result;
        }

        public async Task<ServiceResult> GetCarritoById(int id)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Obteniendo carrito con ID: {id}");

            try
            {
                var opResult = await _carritoRepository.GetEntityBy(id);
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Carrito no encontrado";
                    _logger.LogWarning($"Carrito con ID {id} no encontrado");
                    return result;
                }

                var c = (Carrito)opResult.Data;

                var dto = new CarritoGetDTO
                {
                    CarritoId = c.IdCarrito,
                    ClienteId = c.ClienteId,
                    FechaCreacion = c.FechaCreacion,
                    FechaModificacion = c.FechaModificacion,
                    UsuarioModificacion = c.UsuarioModificacion,
                    Estatus = c.Estatus
                };

                result.Success = true;
                result.Data = dto;
                result.Message = "Carrito obtenido correctamente";
                _logger.LogInformation($"Carrito con ID {id} obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el carrito con ID {id}");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener el carrito";
            }

            return result;
        }

        public async Task<ServiceResult> UpdateCarrito(UpdateCarritoDTO updateCarritoDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Actualizando carrito con ID: {updateCarritoDto.CarritoId}");

            try
            {
                var existingResult = await _carritoRepository.GetEntityBy(updateCarritoDto.CarritoId);
                if (!existingResult.Success || existingResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Carrito no encontrado";
                    return result;
                }

                var c = (Carrito)existingResult.Data;

                c.ClienteId = updateCarritoDto.ClienteId;

                var opResult = await _carritoRepository.Update(c);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = "No se pudo actualizar el carrito";
                    _logger.LogWarning("Error al actualizar carrito");
                    return result;
                }

                result.Success = true;
                result.Message = "Carrito actualizado exitosamente";
                _logger.LogInformation($"Carrito con ID {updateCarritoDto.CarritoId} actualizado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el carrito con ID {updateCarritoDto.CarritoId}");
                result.Success = false;
                result.Message = "Ocurrió un error al actualizar el carrito";
            }

            return result;
        }

        public async Task<ServiceResult> RemoveCarrito(DeleteCarritoDTO deleteCarritoDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Eliminando carrito con ID: {deleteCarritoDto.CarritoId}");

            try
            {
                var existingResult = await _carritoRepository.GetEntityBy(deleteCarritoDto.CarritoId);
                if (!existingResult.Success || existingResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Carrito no encontrado";
                    return result;
                }

                var c = (Carrito)existingResult.Data;

                var opResult = await _carritoRepository.Remove(c);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = "No se pudo eliminar el carrito";
                    _logger.LogWarning("Error al eliminar carrito");
                    return result;
                }

                result.Success = true;
                result.Message = "Carrito eliminado correctamente";
                _logger.LogInformation($"Carrito con ID {deleteCarritoDto.CarritoId} eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el carrito con ID {deleteCarritoDto.CarritoId}");
                result.Success = false;
                result.Message = "Ocurrió un error al eliminar el carrito";
            }

            return result;
        }
    }
}
