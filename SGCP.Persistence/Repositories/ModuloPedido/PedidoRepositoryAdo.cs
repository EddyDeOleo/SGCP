using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloPedido;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityHelper.ModuloPedido;
using SGCP.Persistence.Base.EntityValidator.ModuloPedido;


namespace SGCP.Persistence.Repositories.ModuloPedido
{
    public class PedidoRepositoryAdo : BaseRepositoryAdo<Pedido>, IPedido
    {
        protected override string SpGetAll => "sp_GetAllPedidos";
        protected override string SpGetById => "sp_GetPedidoById";
        protected override string SpInsert => "sp_InsertPedido";
        protected override string SpUpdate => "sp_UpdatePedido";
        protected override string SpDelete => "sp_DeletePedido";

        public PedidoRepositoryAdo(
            IStoredProcedureExecutor spExecutor,
            ILogger<PedidoRepositoryAdo> logger,
            PedidoValidator pedidoValidator)
            : base(spExecutor, logger, pedidoValidator)
        {
        }

        protected override Pedido MapToEntity(SqlDataReader reader)
        {
            var model = PedidoRepositoryHelper.MapToPedidoGetModel(reader);
            return PedidoRepositoryHelper.MapToPedido(model);
        }

        protected override (Dictionary<string, object>, SqlParameter) GetInsertParameters(Pedido entity)
        {
            return PedidoRepositoryHelper.GetInsertParameters(entity);
        }

        protected override Dictionary<string, object> GetUpdateParameters(Pedido entity)
        {
            return PedidoRepositoryHelper.GetUpdateParameters(entity);
        }

        protected override Dictionary<string, object> GetDeleteParameters(Pedido entity)
        {
            return PedidoRepositoryHelper.GetDeleteParameters(entity);
        }

        protected override Dictionary<string, object> GetIdParameter(int id)
        {
            return new Dictionary<string, object> { { "@IdPedido", id } };
        }

        public async Task<List<Pedido>> GetPedidosByClienteId(int clienteId)
        {
            var result = await RepositoryLoggerHelper.ExecuteLoggedAsync<Pedido>(
                _logger,
                nameof(GetPedidosByClienteId),
                async () =>
                {
                    var parameters = new Dictionary<string, object> { { "@ClienteId", clienteId } };

                    var pedidosGet = await _spExecutor.QueryAsync(
                        "sp_GetPedidosByClienteId",
                        MapToEntity,
                        parameters
                    );

                    var pedidos = pedidosGet.ToList();
                    return OperationResult.SuccessResult("Pedidos obtenidos", pedidos);
                },
                clienteId
            );

            return result.Data as List<Pedido> ?? new List<Pedido>();
        }
    }
}
