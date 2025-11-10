using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;

namespace SGCP.Application.Interfaces.ModuloUsuarios
{
    public interface IClienteService
    {
        Task<ServiceResult> GetCliente();

        Task<ServiceResult> GetClienteById(int id);

        Task<ServiceResult> CreateCliente(CreateClienteDTO createClienteDto);

        Task<ServiceResult> UpdateCliente(UpdateClienteDTO updateClienteDto);

        Task<ServiceResult> RemoveCliente(DeleteClienteDTO deleteClienteDto);
    }
}
