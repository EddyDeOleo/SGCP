

using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Services
{
    public sealed class ClienteService : IClienteService
    {
        private readonly ICliente _repository;
        private readonly ILogger<ClienteService> _logger;
        private readonly ISessionService _sessionService;

        public ClienteService(ICliente repository, ILogger<ClienteService> logger, ISessionService sessionService)
        {
            _repository = repository;
            _logger = logger;
            _sessionService = sessionService;
        }

        public async Task<ServiceResult> CreateCliente(CreateClienteDTO createClienteDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation("Creando un nuevo cliente");

            try
            {
                // Validación de username existente...
                var existingResult = await _repository.GetAll();
                if (existingResult.Success && existingResult.Data != null)
                {
                    if (((List<Cliente>)existingResult.Data)
                        .Any(c => c.Username.Equals(createClienteDto.Username, StringComparison.OrdinalIgnoreCase)))
                    {
                        result.Success = false;
                        result.Message = "El username ya está registrado";
                        return result;
                    }
                }

                var cliente = new Cliente(
                
                    createClienteDto.Nombre,
                    createClienteDto.Apellido,
                    createClienteDto.Username,
                    createClienteDto.Password
                );

                // ✅ NO crear el Carrito aquí, dejarlo null
                cliente.Carrito = null;  // Asegurar que sea null

                var opResult = await _repository.Save(cliente);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = opResult.Message;
                    return result;
                }

                result.Success = true;
                result.Message = "Cliente creado correctamente";
                result.Data = cliente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                result.Success = false;
                result.Message = "Ocurrió un error al crear el cliente";
            }

            return result;
        }
        public async Task<ServiceResult> GetCliente()
        {
            var result = new ServiceResult();
            _logger.LogInformation("Obteniendo todos los clientes");

            try
            {
                /*
                // Precondición CU-08: Usuario debe haber iniciado sesión
                if (!_sessionService.ClienteIdLogueado.HasValue)
                {
                    result.Success = false;
                    result.Message = "Debe iniciar sesión para consultar los clientes";
                    return result;
                }
                */

                var opResult = await _repository.GetAll();
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "No se pudieron obtener los clientes";
                    return result;
                }

                var clientesDto = ((List<Cliente>)opResult.Data).Select(c => new ClienteGetDTO
                {
                    ClienteId = c.IdUsuario,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    Username = c.Username,
                    Password = c.Password
                }).ToList();

                result.Success = true;
                result.Message = "Clientes obtenidos correctamente";
                result.Data = clientesDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener los clientes";
            }

            return result;
        }

        public async Task<ServiceResult> GetClienteById(int id)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Obteniendo cliente con ID {id}");

            try
            {
                /*
                // Precondición CU-08: Usuario debe haber iniciado sesión
                if (!_sessionService.ClienteIdLogueado.HasValue)
                {
                    result.Success = false;
                    result.Message = "Debe iniciar sesión para consultar el cliente";
                    return result;
                }
                */

                var opResult = await _repository.GetEntityBy(id);
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Cliente no encontrado";
                    return result;
                }

                var cliente = (Cliente)opResult.Data;

                var clienteDto = new ClienteGetDTO
                {
                    ClienteId = cliente.IdUsuario,
                    Nombre = cliente.Nombre,
                    Apellido = cliente.Apellido,
                    Username = cliente.Username,
                    Password = cliente.Password
                };

                result.Success = true;
                result.Message = "Cliente obtenido correctamente";
                result.Data = clienteDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener cliente con ID {id}");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener el cliente";
            }

            return result;
        }

        public async Task<ServiceResult> UpdateCliente(UpdateClienteDTO updateClienteDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Actualizando cliente con ID {updateClienteDto.ClienteId}");

            try
            {
                /*
                // Precondición CU-08: Usuario debe haber iniciado sesión
                if (!_sessionService.ClienteIdLogueado.HasValue)
                {
                    result.Success = false;
                    result.Message = "Debe iniciar sesión para actualizar el cliente";
                    return result;
                }
                */

                var existingResult = await _repository.GetEntityBy(updateClienteDto.ClienteId);
                if (!existingResult.Success || existingResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Cliente no encontrado";
                    return result;
                }

                var cliente = (Cliente)existingResult.Data;

                cliente.Nombre = updateClienteDto.Nombre;
                cliente.Apellido = updateClienteDto.Apellido;
                cliente.Username = updateClienteDto.Username;
                cliente.Password = updateClienteDto.Password;

                var opResult = await _repository.Update(cliente);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = opResult.Message;
                    return result;
                }

                result.Success = true;
                result.Message = "Cliente actualizado correctamente";
                result.Data = cliente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar cliente con ID {updateClienteDto.ClienteId}");
                result.Success = false;
                result.Message = "Ocurrió un error al actualizar el cliente";
            }

            return result;
        }

        public async Task<ServiceResult> RemoveCliente(DeleteClienteDTO deleteClienteDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Eliminando cliente con ID {deleteClienteDto.ClienteId}");

            try
            {
                /*
                // Precondición CU-08: Usuario debe haber iniciado sesión
                if (!_sessionService.ClienteIdLogueado.HasValue)
                {
                    result.Success = false;
                    result.Message = "Debe iniciar sesión para eliminar el cliente";
                    return result;
                }
                */

                var existingResult = await _repository.GetEntityBy(deleteClienteDto.ClienteId);
                if (!existingResult.Success || existingResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Cliente no encontrado";
                    return result;
                }

                var cliente = (Cliente)existingResult.Data;
                var opResult = await _repository.Remove(cliente);

                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = opResult.Message;
                    return result;
                }

                result.Success = true;
                result.Message = "Cliente eliminado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar cliente con ID {deleteClienteDto.ClienteId}");
                result.Success = false;
                result.Message = "Ocurrió un error al eliminar el cliente";
            }

            return result;
        }
    }
}
