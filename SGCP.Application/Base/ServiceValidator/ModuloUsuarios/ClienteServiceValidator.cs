

using Microsoft.Extensions.Logging;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Base.ServiceValidator.ModuloUsuarios
{

    public class ClienteServiceValidator : ServiceValidator<ClienteServiceValidator>
    {
        private readonly ICliente _clienteRepository;

        public ClienteServiceValidator(
            ILogger<ClienteServiceValidator> logger,
            ICliente clienteRepository
        ) : base(logger)
        {
            _clienteRepository = clienteRepository;
        }

        // Validación para crear cliente
        public ServiceResult ValidateForCreate(CreateClienteDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de creación de cliente");
            if (!dtoVal.Success) return dtoVal;

            if (string.IsNullOrWhiteSpace(dto.Username))
                return Failure("Username no puede estar vacío");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return Failure("Nombre no puede estar vacío");

            if (string.IsNullOrWhiteSpace(dto.Apellido))
                return Failure("Apellido no puede estar vacío");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return Failure("Password no puede estar vacío");

            return Success("DTO válido para crear cliente");
        }

        // Validación para actualizar cliente
        public ServiceResult ValidateForUpdate(UpdateClienteDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de actualización de cliente");
            if (!dtoVal.Success) return dtoVal;

            var idVal = ValidateId(dto.ClienteId, "ClienteId");
            if (!idVal.Success) return idVal;

            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username.Length == 0)
                return Failure("Username no puede estar vacío");

            return Success("DTO válido para actualizar cliente");
        }

        // Validación para eliminar cliente
        public ServiceResult ValidateForDelete(DeleteClienteDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de eliminación de cliente");
            if (!dtoVal.Success) return dtoVal;

            var idVal = ValidateId(dto.ClienteId, "ClienteId");
            if (!idVal.Success) return idVal;

            return Success("DTO válido para eliminar cliente");
        }

        // Validación existencia de cliente en la base
        public async Task<ServiceResult> ValidateClienteExistente(int clienteId)
        {
            var idVal = ValidateId(clienteId, "ClienteId");
            if (!idVal.Success) return idVal;

            var result = await _clienteRepository.GetEntityBy(clienteId);
            if (!result.Success || result.Data == null)
                return Failure("Cliente no existe");

            return Success("Cliente existente", result.Data);
        }
    }

}
