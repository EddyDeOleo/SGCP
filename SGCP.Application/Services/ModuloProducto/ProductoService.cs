
using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Base.ServiceValidator.ModuloProducto;
using SGCP.Application.Dtos.ModuloProducto.Producto;
using SGCP.Application.Interfaces.IServiceValidator.ModuloProducto;
using SGCP.Application.Interfaces.ModuloProducto;
using SGCP.Application.Interfaces.ModuloUsuarios;
using SGCP.Application.Mappers;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Domain.Entities.ModuloDeProducto;

namespace SGCP.Application.Services.ModuloProducto
    {
    public sealed class ProductoService : BaseService<ProductoService>, IProductoService
    {
        private readonly IProducto _productoRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProductoServiceValidator _productoServiceValidator;

        public ProductoService(
            IProducto productoRepository,
            ILogger<ProductoService> logger,
            ICurrentUserService currentUserService,
            IProductoServiceValidator productoServiceValidator
        ) : base(logger)
        {
            _productoRepository = productoRepository;
            _currentUserService = currentUserService;
            _productoServiceValidator = productoServiceValidator;
        }

       
        public async Task<ServiceResult> CreateProducto(CreateProductoDTO dto)
        {
            return await ExecuteSafeAsync("crear producto", async () =>
            {
                var validation = _productoServiceValidator.ValidateForCreate(dto);
                if (!validation.Success) return validation;

                var producto = ProductoMapper.ToEntity(dto);
                var saveResult = await _productoRepository.Save(producto);
                if (!saveResult.Success)
                    return new ServiceResult(false, saveResult.Message);

                return new ServiceResult(true, "Producto creado correctamente", ProductoMapper.ToDto(producto));
            });
        }

        public async Task<ServiceResult> GetProducto()
        {
            return await ExecuteSafeAsync("obtener productos", async () =>
            {
                var opResult = await _productoRepository.GetAll();
                if (!opResult.Success || opResult.Data == null)
                    return new ServiceResult(false, "No se pudieron obtener los productos");

                var productosDto = ((List<Producto>)opResult.Data)
                    .Select(ProductoMapper.ToDto)
                    .ToList();

                return new ServiceResult(true, "Productos obtenidos correctamente", productosDto);
            });
        }

        public async Task<ServiceResult> GetProductoById(int id)
        {
            return await ExecuteSafeAsync($"obtener producto con ID {id}", async () =>
            {
                var validation = await _productoServiceValidator.ValidateProductoExistente(id);
                if (!validation.Success) return validation;

                var producto = (Producto)validation.Data;
                return new ServiceResult(true, "Producto obtenido correctamente", ProductoMapper.ToDto(producto));
            });
        }


        public async Task<ServiceResult> UpdateProducto(UpdateProductoDTO dto)
        {
            return await ExecuteSafeAsync($"actualizar producto con ID {dto.IdProducto}", async () =>
            {
                var validation = _productoServiceValidator.ValidateForUpdate(dto);
                if (!validation.Success) return validation;

                var existing = await _productoServiceValidator.ValidateProductoExistente(dto.IdProducto);
                if (!existing.Success) return existing;

                var producto = (Producto)existing.Data;
                ProductoMapper.MapToEntity(producto, dto);
                producto.UsuarioModificacion = _currentUserService.GetUserId();
                producto.FechaModificacion = DateTime.Now;

                var updateResult = await _productoRepository.Update(producto);
                if (!updateResult.Success)
                    return new ServiceResult(false, updateResult.Message);

                return new ServiceResult(true, "Producto actualizado correctamente", ProductoMapper.ToDto(producto));
            });
        }

        public async Task<ServiceResult> RemoveProducto(DeleteProductoDTO dto)
        {
            return await ExecuteSafeAsync($"eliminar producto con ID {dto.IdProducto}", async () =>
            {
                var validation = _productoServiceValidator.ValidateForDelete(dto);
                if (!validation.Success) return validation;

                var existing = await _productoServiceValidator.ValidateProductoExistente(dto.IdProducto);
                if (!existing.Success) return existing;

                var producto = (Producto)existing.Data;
                var removeResult = await _productoRepository.Remove(producto);

                if (!removeResult.Success)
                    return new ServiceResult(false, removeResult.Message);

                return new ServiceResult(true, "Producto eliminado correctamente (borrado lógico)");
            });
        }
    }

}

