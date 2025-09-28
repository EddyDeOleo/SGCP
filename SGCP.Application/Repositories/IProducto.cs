using SGCP.Domain.Repository;
using SGCP.Domain.Entities.ModuloDeProducto;

namespace SGCP.Application.Repositories
{
    internal interface IProducto : IBaseRepository<Producto>
    {

        List<Producto> GetProductosByCategoria(string categoria);

    }
}
