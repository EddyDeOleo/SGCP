using SGCP.Domain.Repository;
using SGCP.Domain.Entities.ModuloDePedido;

namespace SGCP.Application.Repositories
{
    internal interface IPedido : IBaseRepository<Pedido>
    {
        List<Pedido> GetPedidosByClienteId(int clienteId);

    }
}
