using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityHelper.ModuloCarrito;
using SGCP.Persistence.Base.EntityValidator.ModuloCarrito;



namespace SGCP.Persistence.Repositories.ModuloCarrito
{
    public class CarritoRepositoryAdo : BaseRepositoryAdo<Carrito>, ICarrito
    {
        protected override string SpGetAll => "sp_GetAllCarritos";
        protected override string SpGetById => "sp_GetCarritoById";
        protected override string SpInsert => "sp_InsertCarrito";
        protected override string SpUpdate => "sp_UpdateCarrito";
        protected override string SpDelete => "sp_DeleteCarrito";

        public CarritoRepositoryAdo(
            IStoredProcedureExecutor spExecutor,
            ILogger<CarritoRepositoryAdo> logger,
            CarritoValidator carritoValidator)
            : base(spExecutor, logger, carritoValidator)
        {
        }

        protected override Carrito MapToEntity(SqlDataReader reader)
        {
            var model = CarritoRepositoryHelper.MapToCarritoGetModel(reader);
            return CarritoRepositoryHelper.MapToCarrito(model);
        }

        protected override (Dictionary<string, object>, SqlParameter) GetInsertParameters(Carrito entity)
        {
            return CarritoRepositoryHelper.GetInsertParameters(entity);
        }

        protected override Dictionary<string, object> GetUpdateParameters(Carrito entity)
        {
            return CarritoRepositoryHelper.GetUpdateParameters(entity);
        }

        protected override Dictionary<string, object> GetDeleteParameters(Carrito entity)
        {
            return CarritoRepositoryHelper.GetDeleteParameters(entity);
        }

        protected override Dictionary<string, object> GetIdParameter(int id)
        {
            return new Dictionary<string, object> { { "@IdCarrito", id } };
        }

        public async Task<Carrito?> GetByClienteId(int clienteId)
        {
            var result = await RepositoryLoggerHelper.ExecuteLoggedAsync<Carrito>(
                _logger,
                nameof(GetByClienteId),
                async () =>
                {
                    var results = await _spExecutor.QueryAsync(
                        "sp_GetCarritoByClienteId",
                        MapToEntity,
                        new Dictionary<string, object> { { "@ClienteId", clienteId } }
                    );

                    var carrito = results.FirstOrDefault();
                    if (carrito == null)
                        return OperationResult.FailureResult("No se encontró carrito para el cliente");

                    return OperationResult.SuccessResult("Carrito obtenido", carrito);
                },
                clienteId
            );

            return result.Data as Carrito;
        }
    }
}

