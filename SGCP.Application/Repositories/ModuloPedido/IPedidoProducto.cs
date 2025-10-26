

using SGCP.Domain.Base;
using SGCP.Domain.Repository;

namespace SGCP.Application.Repositories.ModuloPedido
{
    public interface IPedidoProducto 
    {
        Task<OperationResult> AgregarProducto(int pedidoId, int productoId, int cantidad);
    }
}
