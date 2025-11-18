using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityHelper.ModuloProducto;
using SGCP.Persistence.Base.IEntityValidator;


namespace SGCP.Persistence.Repositories.ModuloProducto
{
    public class ProductoRepositoryAdo : BaseRepositoryAdo<Producto>, IProducto
    {
        protected override string SpGetAll => "sp_GetAllProductos";
        protected override string SpGetById => "sp_GetProductoById";
        protected override string SpInsert => "sp_InsertProducto";
        protected override string SpUpdate => "sp_UpdateProducto";
        protected override string SpDelete => "sp_DeleteProducto";

        public ProductoRepositoryAdo(
            IStoredProcedureExecutor spExecutor,
            ILogger<ProductoRepositoryAdo> logger,
            IEntityValidator<Producto> productoValidator)
            : base(spExecutor, logger, productoValidator)
        {
        }

        public override async Task<OperationResult> Save(Producto entity)
        {
            if (_validator != null)
            {
                var validation = _validator.ValidateForSave(entity);
                if (!validation.Success)
                    return validation;
            }

            return await base.Save(entity);
        }

        public override async Task<OperationResult> Update(Producto entity)
        {
            if (_validator != null)
            {
                var validation = _validator.ValidateForUpdate(entity);
                if (!validation.Success)
                    return validation;
            }

            return await base.Update(entity);
        }

        public override async Task<OperationResult> Remove(Producto entity)
        {
            if (_validator != null)
            {
                var validation = _validator.ValidateForRemove(entity);
                if (!validation.Success)
                    return validation;
            }

            return await base.Remove(entity);
        }


        protected override Producto MapToEntity(SqlDataReader reader)
        {
            var model = ProductoRepositoryHelper.MapToProductoGetModel(reader);
            return ProductoRepositoryHelper.MapToProducto(model);
        }

        protected override (Dictionary<string, object>, SqlParameter) GetInsertParameters(Producto entity)
        {

            return ProductoRepositoryHelper.GetInsertParameters(entity);
        }

        protected override Dictionary<string, object> GetUpdateParameters(Producto entity)
        {
            return ProductoRepositoryHelper.GetUpdateParameters(entity);
        }

        protected override Dictionary<string, object> GetDeleteParameters(Producto entity)
        {
            return ProductoRepositoryHelper.GetDeleteParameters(entity);
        }

        protected override Dictionary<string, object> GetIdParameter(int id)
        {
            return new Dictionary<string, object> { { "@IdProducto", id } };
        }


      

        public async Task<List<Producto>> GetProductosByCategoria(string categoria)
        {
            var result = await RepositoryLoggerHelper.ExecuteLoggedAsync<Producto>(
                _logger,
                nameof(GetProductosByCategoria),
                async () =>
                {
                    var parameters = new Dictionary<string, object> { { "@Categoria", categoria } };

                    var productosGet = await _spExecutor.QueryAsync(
                        "sp_GetProductosByCategoria",
                        MapToEntity,
                        parameters
                    );

                    var productos = productosGet.ToList();
                    return OperationResult.SuccessResult("Productos obtenidos", productos);
                },
                categoria
            );

            return result.Data as List<Producto> ?? new List<Producto>();
        }
    }
}

