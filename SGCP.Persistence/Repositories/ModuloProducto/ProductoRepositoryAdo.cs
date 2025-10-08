using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityHelper.ModuloProducto;
using SGCP.Persistence.Base.EntityValidator.ModuloProducto;
using System.Data;
using System.Linq.Expressions;

namespace SGCP.Persistence.Repositories.ModuloProducto
{
    public class ProductoRepositoryAdo : IProducto
    {

        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<ProductoRepositoryAdo> _logger;
        private readonly ProductoValidator _productoValidator;

        public ProductoRepositoryAdo(IStoredProcedureExecutor spExecutor, ILogger<ProductoRepositoryAdo> logger, ProductoValidator productoValidator)
        {
            _spExecutor = spExecutor;
            _logger = logger;
            _productoValidator = productoValidator;
        }
        public async Task<bool> Exists(Expression<Func<Producto, bool>> filter)
        {
            var result = await RepositoryLoggerHelper.ExecuteLoggedAsync<Producto>(
                _logger,
                nameof(Exists),
                async () =>
                {
                    var allResult = await GetAll();
                    if (!allResult.Success || allResult.Data == null)
                        return OperationResult.FailureResult("No se pudo obtener productos para verificar existencia");

                    var productos = ((List<Producto>)allResult.Data).AsQueryable();
                    bool exists = productos.Any(filter.Compile());

                    return OperationResult.SuccessResult("Existencia verificada", exists);
                },
                filter
            );

            return result.Success && result.Data is bool exists && exists;
        }

        public Task<OperationResult> GetAll() =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Producto>(
                _logger,
                nameof(GetAll),
                async () =>
                {
                    var productosGet = await _spExecutor.QueryAsync(
                        "sp_GetAllProductos",
                        ProductoRepositoryHelper.MapToProductoGetModel
                    );

                    var productos = productosGet.Select(ProductoRepositoryHelper.MapToProducto).ToList();
                    return OperationResult.SuccessResult("Productos obtenidos correctamente", productos);
                }
            );

        public Task<OperationResult> GetAll(Expression<Func<Producto, bool>> filter) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Producto>(
                _logger,
                nameof(GetAll),
                async () =>
                {
                    var result = await GetAll();
                    if (!result.Success || result.Data == null)
                        return OperationResult.FailureResult("No se pudieron obtener productos");

                    var productos = ((List<Producto>)result.Data)
                        .AsQueryable()
                        .Where(filter.Compile())
                        .ToList();

                    return OperationResult.SuccessResult("Productos filtrados correctamente", productos);
                },
                filter
            );

        public Task<OperationResult> GetEntityBy(int id) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Producto>(
                _logger,
                nameof(GetEntityBy),
                async () =>
                {
                    var productosGet = await _spExecutor.QueryAsync(
                        "sp_GetProductoById",
                        ProductoRepositoryHelper.MapToProductoGetModel,
                        new Dictionary<string, object> { { "@IdProducto", id } }
                    );

                    if (!productosGet.Any())
                        return OperationResult.FailureResult("Producto no encontrado");

                    var producto = ProductoRepositoryHelper.MapToProducto(productosGet.First());
                    return OperationResult.SuccessResult("Producto obtenido correctamente", producto);
                },
                id
            );

        public Task<List<Producto>> GetProductosByCategoria(string categoria) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Producto>(
                _logger,
                nameof(GetProductosByCategoria),
                async () =>
                {
                    var parameters = new Dictionary<string, object> { { "@Categoria", categoria } };

                    var productosGet = await _spExecutor.QueryAsync(
                        "sp_GetProductosByCategoria",
                        ProductoRepositoryHelper.MapToProductoGetModel,
                        parameters
                    );

                    var productos = productosGet.Select(ProductoRepositoryHelper.MapToProducto).ToList();
                    return OperationResult.SuccessResult("Productos obtenidos", productos);
                },
                categoria
            ).ContinueWith(t => t.Result.Data as List<Producto> ?? new List<Producto>());

        public Task<OperationResult> Save(Producto entity) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Producto>(
                _logger,
                nameof(Save),
                async () =>
                {
                    var validation = _productoValidator.ValidateForSave(entity);
                    if (!validation.Success)
                        return validation;

                    var (parameters, outputParam) = ProductoRepositoryHelper.GetInsertParameters(entity);
                    await _spExecutor.ExecuteAsync("sp_InsertProducto", parameters, outputParam);
                    entity.IdProducto = (int)outputParam.Value;

                    return OperationResult.SuccessResult("Producto creado correctamente", entity);
                },
                entity.Nombre
            );

        public Task<OperationResult> Update(Producto entity) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Producto>(
                _logger,
                nameof(Update),
                async () =>
                {
                    var validation = _productoValidator.ValidateForUpdate(entity);
                    if (!validation.Success) return validation;

                    var parameters = ProductoRepositoryHelper.GetUpdateParameters(entity);
                    await _spExecutor.ExecuteAsync("sp_UpdateProducto", parameters);

                    return OperationResult.SuccessResult("Producto actualizado correctamente", entity);
                },
                entity.IdProducto
            );

        public async Task<OperationResult> Remove(Producto entity)
        {
            return await RepositoryLoggerHelper.ExecuteLoggedAsync<Producto>(
                _logger,
                nameof(Remove),
                async () =>
                {
                    if (entity == null)
                        return OperationResult.FailureResult("El producto no puede ser nulo.");

                    var validation = _productoValidator.ValidateForRemove(entity);
                    if (!validation.Success)
                        return validation;

                    var parameters = ProductoRepositoryHelper.GetDeleteParameters(entity); 
                    await _spExecutor.ExecuteAsync("sp_DeleteProducto", parameters);

                    return OperationResult.SuccessResult("Producto eliminado correctamente");
                },
                entity?.IdProducto
            );
        }
    }
}

