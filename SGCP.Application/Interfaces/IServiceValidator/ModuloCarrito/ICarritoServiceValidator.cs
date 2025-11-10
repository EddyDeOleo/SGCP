

using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloCarrito.Carrito;
using SGCP.Application.Dtos.ModuloCarrito.CarritoProducto;

namespace SGCP.Application.Interfaces.IServiceValidator.ModuloCarrito
{
    public interface ICarritoServiceValidator : IServiceValidatorBase<CreateCarritoDTO, UpdateCarritoDTO, DeleteCarritoDTO>
    {
     
     

        Task<ServiceResult> ValidateCarritoExistente(int carritoId);
        Task<ServiceResult> ValidateProductoExistente(int productoId);


        Task<ServiceResult> ValidateAgregarProductoDTO(int carritoId, AgregarProductoDTO dto);
    }
}
