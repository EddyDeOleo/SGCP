

using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Base.ServiceValidator.ModuloUsuarios;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;
using SGCP.Application.Interfaces;
using SGCP.Application.Mappers;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Services.ModuloUsuarios
{
    public sealed class ClienteService : BaseService<ClienteService>, IClienteService
    {
        private readonly ICliente _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ClienteServiceValidator _clienteServiceValidator;

        public ClienteService(
            ICliente repository,
            ILogger<ClienteService> logger,
            ICurrentUserService currentUserService,
            ClienteServiceValidator clienteServiceValidator
        ) : base(logger)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _clienteServiceValidator = clienteServiceValidator;
        }

        public async Task<ServiceResult> CreateCliente(CreateClienteDTO dto)
        {
            return await ExecuteSafeAsync("crear cliente", async () =>
            {
                var valResult = _clienteServiceValidator.ValidateForCreate(dto);
                if (!valResult.Success) return valResult;

                var existingResult = await _repository.GetAll();
                if (existingResult.Success && existingResult.Data is List<Cliente> clientes)
                {
                    if (clientes.Any(c => c.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase)))
                        return new ServiceResult(false, "El username ya está registrado");
                }

                var cliente = new Cliente(dto.Nombre, dto.Apellido, dto.Username, dto.Password)
                {
                    Carrito = null
                };

                var opResult = await _repository.Save(cliente);
                if (!opResult.Success)
                    return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Cliente creado correctamente", cliente);
            });
        }

        public async Task<ServiceResult> GetCliente()
        {
            return await ExecuteSafeAsync("obtener todos los clientes", async () =>
            {
                var opResult = await _repository.GetAll();
                if (!opResult.Success || opResult.Data == null)
                    return new ServiceResult(false, "No se pudieron obtener los clientes");

                var clientes = ((List<Cliente>)opResult.Data)
                    .Select(ClienteMapper.ToDto)
                    .ToList();

                return new ServiceResult(true, "Clientes obtenidos correctamente", clientes);
            });
        }

        public async Task<ServiceResult> GetClienteById(int id)
        {
            return await ExecuteSafeAsync("obtener cliente", async () =>
            {
                var valResult = await _clienteServiceValidator.ValidateClienteExistente(id);
                if (!valResult.Success) return valResult;

                var cliente = (Cliente)valResult.Data;
                return new ServiceResult(true, "Cliente obtenido correctamente", ClienteMapper.ToDto(cliente));
            });
        }

        public async Task<ServiceResult> UpdateCliente(UpdateClienteDTO dto)
        {
            return await ExecuteSafeAsync("actualizar cliente", async () =>
            {
                var valResult = _clienteServiceValidator.ValidateForUpdate(dto);
                if (!valResult.Success) return valResult;

                var clienteVal = await _clienteServiceValidator.ValidateClienteExistente(dto.ClienteId);
                if (!clienteVal.Success) return clienteVal;

                var cliente = (Cliente)clienteVal.Data;
                ClienteMapper.MapToEntity(cliente, dto, _currentUserService.GetUserId());

                var opResult = await _repository.Update(cliente);
                if (!opResult.Success)
                    return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Cliente actualizado correctamente", cliente);
            });
        }

        public async Task<ServiceResult> RemoveCliente(DeleteClienteDTO dto)
        {
            return await ExecuteSafeAsync("eliminar cliente", async () =>
            {
                var valResult = _clienteServiceValidator.ValidateForDelete(dto);
                if (!valResult.Success) return valResult;

                var clienteVal = await _clienteServiceValidator.ValidateClienteExistente(dto.ClienteId);
                if (!clienteVal.Success) return clienteVal;

                var cliente = (Cliente)clienteVal.Data;
                var opResult = await _repository.Remove(cliente);

                if (!opResult.Success)
                    return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Cliente eliminado correctamente");
            });
        }
    }
}

