using SGCP.Domain.Repository;
using SGCP.Domain.Entities.ModuloDePedido;

namespace SGCP.Application.Repositories.ModuloPedido
{
    public interface IPedido : IBaseRepository<Pedido>
    {
        Task<List<Pedido>> GetPedidosByClienteId(int clienteId);
    }
}
