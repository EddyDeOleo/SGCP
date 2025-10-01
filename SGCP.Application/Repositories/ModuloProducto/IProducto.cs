using SGCP.Domain.Repository;
using SGCP.Domain.Entities.ModuloDeProducto;

namespace SGCP.Application.Repositories.ModuloProducto
{
    public interface IProducto : IBaseRepository<Producto>
    {

            Task<List<Producto>> GetProductosByCategoria(string categoria);
       


    }
}
