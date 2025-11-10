using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;

namespace SGCP.Application.Interfaces.IServiceValidator.ModuloUsuarios
{
    public interface IClienteServiceValidator : IServiceValidatorBase<CreateClienteDTO, UpdateClienteDTO, DeleteClienteDTO>
    {
 
        Task<ServiceResult> ValidateClienteExistente(int clienteId);
        Task<ServiceResult> ValidateUsernameUnico(string username);
    }
}
