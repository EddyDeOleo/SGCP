
using SGCP.Domain.Base;

namespace SGCP.Application.Repositories.ModuloCarrito
{
    public interface ICarritoProducto
    {
        Task<OperationResult> AgregarProducto(int carritoId, int productoId, int cantidad);

        Task<OperationResult> GetProductosByCarritoId(int carritoId);
    }
}
