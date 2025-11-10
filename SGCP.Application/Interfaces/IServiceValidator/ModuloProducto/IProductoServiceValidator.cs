
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloProducto.Producto;

namespace SGCP.Application.Interfaces.IServiceValidator.ModuloProducto
{
    public interface IProductoServiceValidator : IServiceValidatorBase<CreateProductoDTO, UpdateProductoDTO, DeleteProductoDTO>
    {
    
        Task<ServiceResult> ValidateProductoExistente(int idProducto);
    }
}
