using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Repository;

namespace SGCP.Application.Repositories.ModuloCarrito
{
    public interface ICarrito : IBaseRepository<Carrito>
    {
        Task<Carrito> GetByClienteId(int clienteId);

    }
}
