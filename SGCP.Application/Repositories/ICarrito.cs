


using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Repository;

namespace SGCP.Application.Repositories
{
    public interface ICarrito : IBaseRepository<Carrito>
    {
        Carrito GetByClienteId(int clienteId);

    }
}
