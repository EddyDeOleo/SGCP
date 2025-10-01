using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Persistence.Base;
using SGCP.Persistence.Models.ModuloProducto.Producto;
using System.Data;
using System.Linq.Expressions;

namespace SGCP.Persistence.Repositories.ModuloProducto
{
    public class ProductoRepositoryAdo : IProducto
    {

        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<ProductoRepositoryAdo> _logger;

        public ProductoRepositoryAdo(IStoredProcedureExecutor spExecutor, ILogger<ProductoRepositoryAdo> logger)
        {
            _spExecutor = spExecutor;
            _logger = logger;
        }

        public async Task<bool> Exists(Expression<Func<Producto, bool>> filter)
        {
            _logger.LogInformation("Verificando existencia de productos con filtro {Filter}", filter);
            try
            {
                var result = await GetAll();
                if (!result.Success || result.Data == null)
                {
                    _logger.LogWarning("No se pudieron obtener productos para verificar existencia");
                    return false;
                }

                var lista = ((List<ProductoGetModel>)result.Data)
                    .Select(pgm => new Producto(
                        pgm.IdProducto,
                        pgm.Nombre,
                        pgm.Descripcion,
                        pgm.Precio,
                        pgm.Stock,
                        pgm.Categoria
                    ))
                    .AsQueryable();

                bool exists = lista.Any(filter.Compile());
                _logger.LogInformation("Existencia verificada: {Exists}", exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de productos");
                throw;
            }
        }

        public async Task<OperationResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los productos");
            try
            {
                var productos = await _spExecutor.QueryAsync(
                    "sp_GetAllProductos",
                    reader => new ProductoGetModel
                    {
                        IdProducto = reader.GetInt32(reader.GetOrdinal("producto_id")),
                        Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "" : reader.GetString(reader.GetOrdinal("descripcion")),
                        Categoria = reader.IsDBNull(reader.GetOrdinal("categoria")) ? "" : reader.GetString(reader.GetOrdinal("categoria")),
                        Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                        Stock = reader.GetInt32(reader.GetOrdinal("stock"))
                    }
                );

                _logger.LogInformation("{Count} productos obtenidos", productos.Count);
                return OperationResult.SuccessResult("Productos obtenidos correctamente", productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return OperationResult.FailureResult($"Error al obtener productos: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetAll(Expression<Func<Producto, bool>> filter)
        {
            _logger.LogInformation("Obteniendo productos filtrados");
            try
            {
                var result = await GetAll();
                if (!result.Success || result.Data == null)
                {
                    _logger.LogWarning("No se pudieron obtener productos");
                    return OperationResult.FailureResult("No se pudieron obtener productos");
                }

                var productos = ((List<ProductoGetModel>)result.Data)
                    .Select(pgm => new Producto(
                        pgm.IdProducto,
                        pgm.Nombre,
                        pgm.Descripcion,
                        pgm.Precio,
                        pgm.Stock,
                        pgm.Categoria
                    ))
                    .AsQueryable()
                    .Where(filter.Compile())
                    .ToList();

                _logger.LogInformation("{Count} productos filtrados obtenidos", productos.Count);
                return OperationResult.SuccessResult("Productos filtrados correctamente", productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos filtrados");
                return OperationResult.FailureResult($"Error al obtener productos filtrados: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetEntityBy(int id)
        {
            _logger.LogInformation("Obteniendo producto con Id {Id}", id);
            try
            {
                var productos = await _spExecutor.QueryAsync(
                    "sp_GetProductoById",
                    reader => new ProductoGetModel
                    {
                        IdProducto = reader.GetInt32(reader.GetOrdinal("producto_id")),
                        Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "" : reader.GetString(reader.GetOrdinal("descripcion")),
                        Categoria = reader.IsDBNull(reader.GetOrdinal("categoria")) ? "" : reader.GetString(reader.GetOrdinal("categoria")),
                        Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                        Stock = reader.GetInt32(reader.GetOrdinal("stock"))
                    },
                    new Dictionary<string, object> { { "@IdProducto", id } }
                );

                if (!productos.Any())
                {
                    _logger.LogWarning("Producto con Id {Id} no encontrado", id);
                    return OperationResult.FailureResult("Producto no encontrado");
                }

                _logger.LogInformation("Producto con Id {Id} obtenido correctamente", id);
                return OperationResult.SuccessResult("Producto obtenido correctamente", productos.First());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con Id {Id}", id);
                return OperationResult.FailureResult($"Error al obtener producto: {ex.Message}");
            }
        }

        public async Task<List<Producto>> GetProductosByCategoria(string categoria)
        {
            _logger.LogInformation("Obteniendo productos de la categoría {Categoria}", categoria);
            try
            {
                var parameters = new Dictionary<string, object> { { "@Categoria", categoria } };

                var productosGet = await _spExecutor.QueryAsync(
                    "sp_GetProductosByCategoria",
                    reader => new ProductoGetModel
                    {
                        IdProducto = reader.GetInt32(reader.GetOrdinal("producto_id")),
                        Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "" : reader.GetString(reader.GetOrdinal("descripcion")),
                        Categoria = reader.IsDBNull(reader.GetOrdinal("categoria")) ? "" : reader.GetString(reader.GetOrdinal("categoria")),
                        Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                        Stock = reader.GetInt32(reader.GetOrdinal("stock"))
                    },
                    parameters
                );

                var productos = productosGet.Select(pgm => new Producto(
                    pgm.IdProducto,
                    pgm.Nombre,
                    pgm.Descripcion,
                    pgm.Precio,
                    pgm.Stock,
                    pgm.Categoria
                )).ToList();

                _logger.LogInformation("{Count} productos obtenidos de la categoría {Categoria}", productos.Count, categoria);
                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos de la categoría {Categoria}", categoria);
                throw;
            }
        }

        public async Task<OperationResult> Save(Producto entity)
        {
            _logger.LogInformation("Creando producto {Nombre}", entity?.Nombre);
            if (entity == null)
            {
                _logger.LogWarning("Producto nulo no puede ser guardado");
                return OperationResult.FailureResult("El producto no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(entity.Nombre) || entity.Nombre.Length > 100)
            {
                _logger.LogWarning("Nombre inválido para el producto");
                return OperationResult.FailureResult("El nombre del producto es obligatorio y no puede exceder 100 caracteres.");
            }
            if (!string.IsNullOrEmpty(entity.Descripcion) && entity.Descripcion.Length > 255)
            {
                _logger.LogWarning("Descripción demasiado larga para el producto");
                return OperationResult.FailureResult("La descripción no puede exceder 255 caracteres.");
            }
            if (!string.IsNullOrEmpty(entity.Categoria) && entity.Categoria.Length > 50)
            {
                _logger.LogWarning("Categoría demasiado larga para el producto");
                return OperationResult.FailureResult("La categoría no puede exceder 50 caracteres.");
            }
            if (entity.Precio <= 0)
            {
                _logger.LogWarning("Precio inválido para el producto");
                return OperationResult.FailureResult("El precio debe ser mayor a cero.");
            }
            if (entity.Stock < 0)
            {
                _logger.LogWarning("Stock negativo no permitido para el producto");
                return OperationResult.FailureResult("El stock no puede ser negativo.");
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@Nombre", entity.Nombre },
                    { "@Descripcion", entity.Descripcion },
                    { "@Categoria", entity.Categoria },
                    { "@Precio", entity.Precio },
                    { "@Stock", entity.Stock }
                };

                var outputParam = new SqlParameter("@IdProducto", SqlDbType.Int) { Direction = ParameterDirection.Output };
                int rowsAffected = await _spExecutor.ExecuteAsync("sp_InsertProducto", parameters, outputParam);

                if (rowsAffected > 0)
                {
                    entity.IdProducto = (int)outputParam.Value;
                    _logger.LogInformation("Producto creado correctamente con Id {IdProducto}", entity.IdProducto);
                    return OperationResult.SuccessResult("Producto creado correctamente.", entity);
                }
                else
                {
                    _logger.LogWarning("No se pudo crear el producto");
                    return OperationResult.FailureResult("No se pudo crear el producto.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto {Nombre}", entity.Nombre);
                return OperationResult.FailureResult($"Error al crear producto: {ex.Message}");
            }
        }

        public async Task<OperationResult> Update(Producto entity)
        {
            _logger.LogInformation("Actualizando producto Id {IdProducto}", entity?.IdProducto);
            if (entity == null)
            {
                _logger.LogWarning("Producto nulo no puede ser actualizado");
                return OperationResult.FailureResult("El producto no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(entity.Nombre) || entity.Nombre.Length > 100 ||
                (!string.IsNullOrEmpty(entity.Descripcion) && entity.Descripcion.Length > 255) ||
                (!string.IsNullOrEmpty(entity.Categoria) && entity.Categoria.Length > 50) ||
                entity.Precio <= 0 || entity.Stock < 0)
            {
                _logger.LogWarning("Datos inválidos para actualizar producto Id {IdProducto}", entity.IdProducto);
                return OperationResult.FailureResult("Datos inválidos para actualizar el producto.");
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@IdProducto", entity.IdProducto },
                    { "@Nombre", entity.Nombre },
                    { "@Descripcion", entity.Descripcion },
                    { "@Categoria", entity.Categoria },
                    { "@Precio", entity.Precio },
                    { "@Stock", entity.Stock }
                };

                await _spExecutor.ExecuteAsync("sp_UpdateProducto", parameters);
                _logger.LogInformation("Producto actualizado correctamente Id {IdProducto}", entity.IdProducto);
                return OperationResult.SuccessResult("Producto actualizado correctamente", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto Id {IdProducto}", entity.IdProducto);
                return OperationResult.FailureResult($"Error al actualizar producto: {ex.Message}");
            }
        }

        public async Task<OperationResult> Remove(Producto entity)
        {
            _logger.LogInformation("Eliminando producto Id {IdProducto}", entity?.IdProducto);
            if (entity == null)
            {
                _logger.LogWarning("Producto nulo no puede ser eliminado");
                return OperationResult.FailureResult("El producto no puede ser nulo.");
            }

            try
            {
                var parameters = new Dictionary<string, object> { { "@IdProducto", entity.IdProducto } };
                await _spExecutor.ExecuteAsync("sp_DeleteProducto", parameters);
                _logger.LogInformation("Producto eliminado correctamente Id {IdProducto}", entity.IdProducto);
                return OperationResult.SuccessResult("Producto eliminado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto Id {IdProducto}", entity.IdProducto);
                return OperationResult.FailureResult($"Error al eliminar producto: {ex.Message}");
            }
        }
        }
}
