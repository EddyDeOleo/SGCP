using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeCarrito;


namespace SGCP.Persistence.Base.EntityValidator.ModuloCarrito
{
    public class CarritoValidator : EntityValidator<Carrito>
    {
        public CarritoValidator(ILogger<CarritoValidator> logger) : base(logger) { }

        public override OperationResult ValidateForSave(Carrito entity)
        {
            return ValidateBase(entity); 
        }

        public override OperationResult ValidateForUpdate(Carrito entity)
        {
            return ValidateBase(entity); 
        }

        public override OperationResult ValidateForRemove(Carrito entity)
        {
            return ValidateBase(entity); 
        }
    }
}
