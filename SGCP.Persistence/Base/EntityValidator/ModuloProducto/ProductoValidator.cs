using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeProducto;


namespace SGCP.Persistence.Base.EntityValidator.ModuloProducto
{
    public class ProductoValidator : EntityValidator<Producto>
    {
        public ProductoValidator(ILogger<ProductoValidator> logger) : base(logger) { }

        public override OperationResult ValidateForSave(Producto entity)
        {
            var baseResult = ValidateBase(entity);
            if (!baseResult.Success)
                return baseResult;

            if (string.IsNullOrWhiteSpace(entity.Nombre) || entity.Nombre.Length > 100)
                return OperationResult.FailureResult("El nombre del producto es obligatorio y no puede exceder 100 caracteres.");

            if (!string.IsNullOrEmpty(entity.Descripcion) && entity.Descripcion.Length > 255)
                return OperationResult.FailureResult("La descripción no puede exceder 255 caracteres.");

            if (!string.IsNullOrEmpty(entity.Categoria) && entity.Categoria.Length > 50)
                return OperationResult.FailureResult("La categoría no puede exceder 50 caracteres.");

            if (entity.Precio <= 0)
                return OperationResult.FailureResult("El precio debe ser mayor a cero.");

            if (entity.Stock < 0)
                return OperationResult.FailureResult("El stock no puede ser negativo.");

            return OperationResult.SuccessResult("Validación de guardado exitosa.");
        }

        public override OperationResult ValidateForUpdate(Producto entity)
        {
            var result = ValidateForSave(entity);
            if (!result.Success)
                return result;

            if (entity.IdProducto <= 0)
                return OperationResult.FailureResult("El Id del producto debe ser válido para actualizar.");

            return OperationResult.SuccessResult("Validación de actualización exitosa.");
        }

        public override OperationResult ValidateForRemove(Producto entity)
        {
            if (entity == null)
                return OperationResult.FailureResult("El producto no puede ser nulo.");

            if (entity.IdProducto <= 0)
                return OperationResult.FailureResult("El Id del producto debe ser válido para eliminar.");

            return OperationResult.SuccessResult("Validación de eliminación exitosa.");
        }

    }
}
