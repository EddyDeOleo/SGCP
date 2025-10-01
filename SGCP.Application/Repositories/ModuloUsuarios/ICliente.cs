using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Domain.Repository;

namespace SGCP.Application.Repositories.ModuloUsuarios
{
    public interface ICliente : IBaseRepository<Cliente>
    {
        Task<Cliente> GetByUsername(string username);

        Task<List<Pedido>> GetPedidosByClienteId(int clienteId);
    }
}
