

using Microsoft.Extensions.Logging;
using SGCP.Application.Dtos.ModuloProducto.Producto;
using SGCP.Application.Interfaces.IServiceValidator.ModuloProducto;
using SGCP.Application.Repositories.ModuloProducto;

namespace SGCP.Application.Base.ServiceValidator.ModuloProducto
{
    public class ProductoServiceValidator : ServiceValidator<ProductoServiceValidator>, IProductoServiceValidator
    {
        private readonly IProducto _productoRepository;

        public ProductoServiceValidator(
            ILogger<ProductoServiceValidator> logger,
            IProducto productoRepository
        ) : base(logger)
        {
            _productoRepository = productoRepository;
        }

      
        public ServiceResult ValidateForCreate(CreateProductoDTO dto)
        {
            var nombreVal = ValidateNotNull(dto.Nombre, "Nombre del producto");
            if (!nombreVal.Success) return nombreVal;

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return Failure("El nombre del producto no puede estar vacío.");

            if (dto.Precio < 0)
                return Failure("El precio del producto no puede ser negativo.");

            if (dto.Stock < 0)
                return Failure("El stock del producto no puede ser negativo.");

            return Success("DTO válido para crear producto");
        }

        public ServiceResult ValidateForUpdate(UpdateProductoDTO dto)
        {
            var idVal = ValidateId(dto.IdProducto, "IdProducto");
            if (!idVal.Success) return idVal;

            return ValidateForCreate(new CreateProductoDTO
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Stock = dto.Stock
            });
        }

        public ServiceResult ValidateForDelete(DeleteProductoDTO dto)
        {
            return ValidateId(dto.IdProducto, "IdProducto");
        }

 
        public async Task<ServiceResult> ValidateProductoExistente(int idProducto)
        {
            var idVal = ValidateId(idProducto, "IdProducto");
            if (!idVal.Success) return idVal;

            var result = await _productoRepository.GetEntityBy(idProducto);
            if (!result.Success || result.Data == null)
                return Failure("El producto no existe.");

            return Success("Producto existente", result.Data);
        }
    }
}
