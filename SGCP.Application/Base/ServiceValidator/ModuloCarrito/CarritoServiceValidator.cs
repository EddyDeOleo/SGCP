

using Microsoft.Extensions.Logging;
using SGCP.Application.Dtos.ModuloCarrito.Carrito;
using SGCP.Application.Dtos.ModuloCarrito.CarritoProducto;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloProducto;

namespace SGCP.Application.Base.ServiceValidator.ModuloCarrito
{
    public class CarritoServiceValidator : ServiceValidator<CarritoServiceValidator>
    {
        private readonly ICarrito _carritoRepository;
        private readonly IProducto _productoRepository;

        public CarritoServiceValidator(
            ILogger<CarritoServiceValidator> logger,
            ICarrito carritoRepository,
            IProducto productoRepository
        ) : base(logger)
        {
            _carritoRepository = carritoRepository;
            _productoRepository = productoRepository;
        }

        public ServiceResult ValidateForCreate(CreateCarritoDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de creación de carrito");
            if (!dtoVal.Success) return dtoVal;

            return Success("DTO válido para crear carrito");
        }

        public ServiceResult ValidateForUpdate(UpdateCarritoDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de actualización de carrito");
            if (!dtoVal.Success) return dtoVal;

            var idVal = ValidateId(dto.CarritoId, "CarritoId");
            if (!idVal.Success) return idVal;

            return Success("DTO válido para actualizar carrito");
        }

        public ServiceResult ValidateForDelete(DeleteCarritoDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de eliminación de carrito");
            if (!dtoVal.Success) return dtoVal;

            var idVal = ValidateId(dto.CarritoId, "CarritoId");
            if (!idVal.Success) return idVal;

            return Success("DTO válido para eliminar carrito");
        }

        public async Task<ServiceResult> ValidateCarritoExistente(int carritoId)
        {
            var idVal = ValidateId(carritoId, "CarritoId");
            if (!idVal.Success) return idVal;

            var result = await _carritoRepository.GetEntityBy(carritoId);
            if (!result.Success || result.Data == null)
                return Failure("Carrito no existe");

            return Success("Carrito existente", result.Data);
        }

        public async Task<ServiceResult> ValidateProductoExistente(int productoId)
        {
            var idVal = ValidateId(productoId, "ProductoId");
            if (!idVal.Success) return idVal;

            var result = await _productoRepository.GetEntityBy(productoId);
            if (!result.Success || result.Data == null)
                return Failure("Producto no existe");

            return Success("Producto válido", result.Data);
        }

        public async Task<ServiceResult> ValidateAgregarProductoDTO(int carritoId, AgregarProductoDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de agregar producto al carrito");
            if (!dtoVal.Success) return dtoVal;

            if (dto.Cantidad <= 0)
                return Failure("La cantidad debe ser mayor a cero");

            var carritoVal = await ValidateCarritoExistente(carritoId);
            if (!carritoVal.Success) return carritoVal;

            var productoVal = await ValidateProductoExistente(dto.ProductoId);
            if (!productoVal.Success) return productoVal;

            var producto = (Domain.Entities.ModuloDeProducto.Producto)productoVal.Data;
            if (producto.Stock < dto.Cantidad)
                return Failure($"Stock insuficiente. Disponible: {producto.Stock}, Solicitado: {dto.Cantidad}");

            return Success("DTO válido para agregar producto al carrito");
        }
    }
}
