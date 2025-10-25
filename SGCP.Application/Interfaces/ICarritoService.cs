
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloCarrito.Carrito;
using SGCP.Application.Dtos.ModuloCarrito.CarritoProducto;

namespace SGCP.Application.Interfaces
{
    public interface ICarritoService 
    {
        Task<ServiceResult> GetCarrito();

        Task<ServiceResult> GetCarritoById(int id);

        Task<ServiceResult> CreateCarrito(CreateCarritoDTO createCarritoDto);

        Task<ServiceResult> UpdateCarrito(UpdateCarritoDTO updateCarritoDto);

        Task<ServiceResult> RemoveCarrito(DeleteCarritoDTO deleteCarritoDto);

        Task<ServiceResult> AgregarProductoAlCarrito(int carritoId, AgregarProductoDTO dto);
    }
}
