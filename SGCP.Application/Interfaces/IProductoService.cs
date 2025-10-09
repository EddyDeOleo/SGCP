
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloProducto.Producto;

namespace SGCP.Application.Interfaces
{
    internal interface IProductoService
    {

        Task<ServiceResult> GetProducto();

        Task<ServiceResult> GetProductoById(int id);

        Task<ServiceResult> CreateProducto(CreateProductoDTO createProductoDto);

        Task<ServiceResult> UpdateProducto(UpdateProductoDTO updateProductoDto);

        Task<ServiceResult> RemoveProducto(DeleteProductoDTO deleteProductoDto);
    }
}
