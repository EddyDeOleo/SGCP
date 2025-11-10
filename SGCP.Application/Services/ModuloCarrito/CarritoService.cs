

using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Base.ServiceValidator.ModuloCarrito;
using SGCP.Application.Dtos.ModuloCarrito.Carrito;
using SGCP.Application.Dtos.ModuloCarrito.CarritoProducto;
using SGCP.Application.Interfaces;
using SGCP.Application.Mappers;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDeProducto;

namespace SGCP.Application.Services.ModuloCarrito
    {
    public sealed class CarritoService : BaseService<CarritoService>, ICarritoService
    {
        private readonly ICarrito _carritoRepository;
        private readonly ICarritoProducto _carritoProductoRepo;
        private readonly IProducto _productoRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly CarritoServiceValidator _carritoServiceValidator; 

        public CarritoService(
            ICarrito carritoRepository,
            ICarritoProducto carritoProductoRepo,
            ILogger<CarritoService> logger,
            IProducto productoRepository,
            ICurrentUserService currentUserService,
            CarritoServiceValidator carritoServiceValidator 
        ) : base(logger)
        {
            _carritoRepository = carritoRepository;
            _carritoProductoRepo = carritoProductoRepo;
            _productoRepository = productoRepository;
            _currentUserService = currentUserService;
            _carritoServiceValidator = carritoServiceValidator; 
        }

        public async Task<ServiceResult> AgregarProductoAlCarrito(int carritoId, AgregarProductoDTO dto)
        {
            return await ExecuteSafeAsync($"agregar producto {dto.ProductoId} al carrito {carritoId}", async () =>
            {
                var validationResult = await _carritoServiceValidator.ValidateAgregarProductoDTO(carritoId, dto);
                if (!validationResult.Success)
                    return validationResult;

                var addResult = await _carritoProductoRepo.AgregarProducto(carritoId, dto.ProductoId, dto.Cantidad);
                return new ServiceResult(addResult.Success, addResult.Message);
            });
        }

        public async Task<ServiceResult> CreateCarrito(CreateCarritoDTO dto)
        {
            return await ExecuteSafeAsync("crear el carrito", async () =>
            {
                var validationResult = _carritoServiceValidator.ValidateForCreate(dto);
                if (!validationResult.Success) return validationResult;

                var carrito = CarritoMapper.ToEntity(dto);
                var opResult = await _carritoRepository.Save(carrito);

                if (!opResult.Success)
                    return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Carrito creado correctamente", CarritoMapper.ToDto(carrito));
            });
        }

        public async Task<ServiceResult> GetCarrito()
        {
            return await ExecuteSafeAsync("obtener los carritos", async () =>
            {
                var opResult = await _carritoRepository.GetAll();
                if (!opResult.Success || opResult.Data == null)
                    return new ServiceResult(false, "No se pudieron obtener los carritos");

                var carritosDto = ((List<Carrito>)opResult.Data)
                    .Select(CarritoMapper.ToDto)
                    .ToList();

                return new ServiceResult(true, "Carritos obtenidos correctamente", carritosDto);
            });
        }

        public async Task<ServiceResult> GetCarritoById(int id)
        {
            return await ExecuteSafeAsync($"obtener carrito con ID {id}", async () =>
            {
                var validationResult = await _carritoServiceValidator.ValidateCarritoExistente(id);
                if (!validationResult.Success) return validationResult;

                var carrito = (Carrito)validationResult.Data;
                return new ServiceResult(true, "Carrito obtenido correctamente", CarritoMapper.ToDto(carrito));
            });
        }

        public async Task<ServiceResult> UpdateCarrito(UpdateCarritoDTO dto)
        {
            return await ExecuteSafeAsync($"actualizar carrito con ID {dto.CarritoId}", async () =>
            {
                var validationResult = _carritoServiceValidator.ValidateForUpdate(dto);
                if (!validationResult.Success) return validationResult;

                var existingResult = await _carritoServiceValidator.ValidateCarritoExistente(dto.CarritoId);
                if (!existingResult.Success) return existingResult;

                var carrito = (Carrito)existingResult.Data;
                CarritoMapper.MapToEntity(carrito, dto);
                carrito.UsuarioModificacion = _currentUserService.GetUserId();

                var opResult = await _carritoRepository.Update(carrito);
                if (!opResult.Success)
                    return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Carrito actualizado correctamente", CarritoMapper.ToDto(carrito));
            });
        }

        public async Task<ServiceResult> RemoveCarrito(DeleteCarritoDTO dto)
        {
            return await ExecuteSafeAsync($"eliminar carrito con ID {dto.CarritoId}", async () =>
            {
                var validationResult = _carritoServiceValidator.ValidateForDelete(dto);
                if (!validationResult.Success) return validationResult;

                var existingResult = await _carritoServiceValidator.ValidateCarritoExistente(dto.CarritoId);
                if (!existingResult.Success) return existingResult;

                var carrito = (Carrito)existingResult.Data;
                var opResult = await _carritoRepository.Remove(carrito);
                if (!opResult.Success)
                    return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Carrito eliminado correctamente");
            });
        }
    }
}



