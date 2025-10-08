using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloProducto.Producto;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Domain.Entities.ModuloDeProducto;

namespace SGCP.Application.Services
{
    public sealed class ProductoService : IProductoService
    {
        
    private readonly IProducto _productoRepository;
    private readonly ILogger<ProductoService> _logger;

    public ProductoService(IProducto productoRepository, ILogger<ProductoService> logger)
    {
        _productoRepository = productoRepository;
        _logger = logger;
    }

     
        public async Task<ServiceResult> CreateProducto(CreateProductoDTO createProductoDto)
        {
            ServiceResult result = new ServiceResult();

            _logger.LogInformation("Iniciando la creación de un nuevo producto");

            try
            {
                // Validaciones de negocio

                if (string.IsNullOrWhiteSpace(createProductoDto.Nombre))
                {
                    result.Success = false;
                    result.Message = "El nombre del producto es obligatorio";
                    return result;
                }

                _logger.LogInformation("Validaciones de negocio completadas exitosamente");

                Producto producto = new Producto
                {
                    Nombre = createProductoDto.Nombre,
                    Descripcion = createProductoDto.Descripcion,
                    Categoria = createProductoDto.Categoria,
                    Precio = createProductoDto.Precio,
                    Stock = createProductoDto.Stock
                };

                var opResult = await _productoRepository.Save(producto);
                if (!opResult.Success)
                {
                    _logger.LogWarning("No se pudo guardar el producto en la base de datos.");
                    result.Success = false;
                    result.Message = opResult.Message;
                    return result;
                }

                result.Success = opResult.Success;
                result.Message = opResult.Message;
                result.Data = producto;

                _logger.LogInformation($"Producto creado exitosamente con ID: {producto.IdProducto}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear el producto", ex.ToString());
                result.Success = false;
                result.Message = "Ocurrió un error al crear el producto";
            }

            return result;
        }
        public async Task<ServiceResult> GetProductos()
        {
            ServiceResult result = new ServiceResult();
            _logger.LogInformation("Obteniendo todos los productos");

            try
            {
                var opResult = await _productoRepository.GetAll();

                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "No se pudieron obtener los productos";
                    _logger.LogWarning("Error al obtener productos");
                    return result;
                }

                result.Success = true;
                result.Data = opResult.Data;
                result.Message = "Productos obtenidos correctamente";
                _logger.LogInformation("Productos obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener los productos", ex.ToString());
                result.Success = false;
                result.Message = "Ocurrió un error al obtener los productos";
            }

            return result;
        }

        public async Task<ServiceResult> GetProductoById(int id)
        {
            ServiceResult result = new ServiceResult();
            _logger.LogInformation($"Obteniendo producto con ID: {id}");

            try
            {
                var opResult = await _productoRepository.GetEntityBy(id);

                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Producto no encontrado";
                    _logger.LogWarning($"Producto con ID {id} no encontrado");
                    return result;
                }

                result.Success = true;
                result.Data = opResult.Data;
                result.Message = "Producto obtenido correctamente";
                _logger.LogInformation($"Producto con ID {id} obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el producto con ID {id}", ex.ToString());
                result.Success = false;
                result.Message = "Ocurrió un error al obtener el producto";
            }

            return result;
        }

        public async Task<ServiceResult> UpdateProducto(UpdateProductoDTO updateProductoDto)
        {
            ServiceResult result = new ServiceResult();
            _logger.LogInformation($"Iniciando actualización del producto con ID: {updateProductoDto.IdProducto}");

            try
            {
                var existingResult = await _productoRepository.GetEntityBy(updateProductoDto.IdProducto);
                if (!existingResult.Success || existingResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Producto no encontrado";
                    _logger.LogWarning($"Producto con ID {updateProductoDto.IdProducto} no encontrado");
                    return result;
                }

                Producto productoExistente = (Producto)existingResult.Data;

                productoExistente.Nombre = updateProductoDto.Nombre;
                productoExistente.Descripcion = updateProductoDto.Descripcion;
                productoExistente.Categoria = updateProductoDto.Categoria;
                productoExistente.Precio = updateProductoDto.Precio;
                productoExistente.Stock = updateProductoDto.Stock;

                var opResult = await _productoRepository.Update(productoExistente);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = opResult.Message;
                    _logger.LogWarning($"No se pudo actualizar el producto con ID {updateProductoDto.IdProducto}");
                    return result;
                }

                result.Success = true;
                result.Message = "Producto actualizado correctamente";
                result.Data = productoExistente;
                _logger.LogInformation($"Producto con ID {updateProductoDto.IdProducto} actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el producto con ID {updateProductoDto.IdProducto}", ex.ToString());
                result.Success = false;
                result.Message = "Ocurrió un error al actualizar el producto";
            }

            return result;
        }

        public async Task<ServiceResult> RemoveProducto(DeleteProductoDTO deleteProductoDto)
        {
            ServiceResult result = new ServiceResult();
            _logger.LogInformation($"Iniciando eliminación del producto con ID: {deleteProductoDto.IdProducto}");

            try
            {
                var existingResult = await _productoRepository.GetEntityBy(deleteProductoDto.IdProducto);
                if (!existingResult.Success || existingResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Producto no encontrado";
                    _logger.LogWarning($"Producto con ID {deleteProductoDto.IdProducto} no encontrado");
                    return result;
                }

                Producto productoExistente = (Producto)existingResult.Data;

                var opResult = await _productoRepository.Remove(productoExistente);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = opResult.Message;
                    _logger.LogWarning($"No se pudo eliminar el producto con ID {deleteProductoDto.IdProducto}");
                    return result;
                }

                result.Success = true;
                result.Message = "Producto eliminado correctamente";
                _logger.LogInformation($"Producto con ID {deleteProductoDto.IdProducto} eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el producto con ID {deleteProductoDto.IdProducto}", ex.ToString());
                result.Success = false;
                result.Message = "Ocurrió un error al eliminar el producto";
            }

            return result;
        }
    }

    }

