

using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloCarrito.Carrito;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Domain.Entities.ModuloDeCarrito;

namespace SGCP.Application.Services
    {
        public sealed class CarritoService : ICarritoService
        {
            private readonly ICarrito _carritoRepository;
            private readonly ILogger<CarritoService> _logger;
            private readonly ISessionService _sessionService; 

            public CarritoService(ICarrito carritoRepository, ILogger<CarritoService> logger, ISessionService sessionService)
            {
                _carritoRepository = carritoRepository;
                _logger = logger;
                _sessionService = sessionService;
            }

            public async Task<ServiceResult> CreateCarrito(CreateCarritoDTO createCarritoDto)
            {
                var result = new ServiceResult();
                _logger.LogInformation("Creando nuevo carrito");

                try
                {
                    // CU-01: Verificar sesión de cliente
                    if (!_sessionService.ClienteIdLogueado.HasValue)
                    {
                        result.Success = false;
                        result.Message = "Debe iniciar sesión para crear un carrito";
                        return result;
                    }

                    var carrito = new Carrito
                    {
                        ClienteId = _sessionService.ClienteIdLogueado.Value
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
                        ClienteId = carrito.ClienteId
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
                    if (!_sessionService.ClienteIdLogueado.HasValue)
                    {
                        result.Success = false;
                        result.Message = "Debe iniciar sesión para consultar carritos";
                        return result;
                    }

                    var opResult = await _carritoRepository.GetAll();

                    if (!opResult.Success || opResult.Data == null)
                    {
                        result.Success = false;
                        result.Message = "No se pudieron obtener los carritos";
                        _logger.LogWarning("Error al obtener carritos");
                        return result;
                    }

                    var carritos = ((List<Carrito>)opResult.Data)
                        .Where(c => c.ClienteId == _sessionService.ClienteIdLogueado.Value) 
                        .Select(c => new CarritoGetDTO
                        {
                            CarritoId = c.IdCarrito,
                            ClienteId = c.ClienteId
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
                    if (!_sessionService.ClienteIdLogueado.HasValue)
                    {
                        result.Success = false;
                        result.Message = "Debe iniciar sesión para consultar carritos";
                        return result;
                    }

                    var opResult = await _carritoRepository.GetEntityBy(id);
                    if (!opResult.Success || opResult.Data == null)
                    {
                        result.Success = false;
                        result.Message = "Carrito no encontrado";
                        _logger.LogWarning($"Carrito con ID {id} no encontrado");
                        return result;
                    }

                    var c = (Carrito)opResult.Data;

                    // Validar que el carrito pertenece al cliente logueado
                    if (c.ClienteId != _sessionService.ClienteIdLogueado.Value)
                    {
                        result.Success = false;
                        result.Message = "No tiene permiso para ver este carrito";
                        return result;
                    }

                    var dto = new CarritoGetDTO
                    {
                        CarritoId = c.IdCarrito,
                        ClienteId = c.ClienteId
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
                    if (!_sessionService.ClienteIdLogueado.HasValue)
                    {
                        result.Success = false;
                        result.Message = "Debe iniciar sesión para actualizar carritos";
                        return result;
                    }

                    // Validar que el carrito pertenece al cliente logueado
                    var existingResult = await _carritoRepository.GetEntityBy(updateCarritoDto.CarritoId);
                    if (!existingResult.Success || existingResult.Data == null)
                    {
                        result.Success = false;
                        result.Message = "Carrito no encontrado";
                        return result;
                    }

                    var c = (Carrito)existingResult.Data;
                    if (c.ClienteId != _sessionService.ClienteIdLogueado.Value)
                    {
                        result.Success = false;
                        result.Message = "No tiene permiso para actualizar este carrito";
                        return result;
                    }

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
                    if (!_sessionService.ClienteIdLogueado.HasValue)
                    {
                        result.Success = false;
                        result.Message = "Debe iniciar sesión para eliminar carritos";
                        return result;
                    }

                    var existingResult = await _carritoRepository.GetEntityBy(deleteCarritoDto.CarritoId);
                    if (!existingResult.Success || existingResult.Data == null)
                    {
                        result.Success = false;
                        result.Message = "Carrito no encontrado";
                        return result;
                    }

                    var c = (Carrito)existingResult.Data;
                    if (c.ClienteId != _sessionService.ClienteIdLogueado.Value)
                    {
                        result.Success = false;
                        result.Message = "No tiene permiso para eliminar este carrito";
                        return result;
                    }

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
