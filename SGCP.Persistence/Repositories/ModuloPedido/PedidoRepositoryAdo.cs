using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloPedido;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityHelper.ModuloPedido;
using SGCP.Persistence.Base.EntityValidator.ModuloPedido;
using System.Data;
using System.Linq.Expressions;

namespace SGCP.Persistence.Repositories.ModuloPedido
{
    public class PedidoRepositoryAdo : IPedido
    {

        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<PedidoRepositoryAdo> _logger;
        private readonly PedidoValidator _pedidoValidator;


        public PedidoRepositoryAdo(IStoredProcedureExecutor spExecutor, ILogger<PedidoRepositoryAdo> logger, PedidoValidator pedidoValidator)
        {
            _spExecutor = spExecutor;
            _logger = logger;
            _pedidoValidator = pedidoValidator;
        }
        public async Task<bool> Exists(Expression<Func<Pedido, bool>> filter)
        {
            var result = await RepositoryLoggerHelper.ExecuteLoggedAsync<Pedido>(
                _logger,
                nameof(Exists),
                async () =>
                {
                    var allResult = await GetAll();

                    if (!allResult.Success || allResult.Data == null)
                        return OperationResult.FailureResult("No se pudo obtener pedidos para verificar existencia");

                    var pedidos = ((List<Pedido>)allResult.Data).AsQueryable();

                    bool exists = pedidos.Any(filter.Compile());

                    return OperationResult.SuccessResult("Existencia verificada", exists);
                },
                filter
            );

            return result.Success && result.Data is bool b && b;
        }


        public Task<OperationResult> GetAll() =>
    RepositoryLoggerHelper.ExecuteLoggedAsync<Pedido>(
        _logger,
        nameof(GetAll),
        async () =>
        {
            var pedidosGet = await _spExecutor.QueryAsync(
                "sp_GetAllPedidos",
                PedidoRepositoryHelper.MapToPedidoGetModel
            );

            var pedidos = pedidosGet.Select(PedidoRepositoryHelper.MapToPedido).ToList();
            return OperationResult.SuccessResult("Pedidos obtenidos correctamente", pedidos);
        }
    );


        public Task<OperationResult> GetEntityBy(int id) =>
    RepositoryLoggerHelper.ExecuteLoggedAsync<Pedido>(
        _logger,
        nameof(GetEntityBy),
        async () =>
        {
            var pedidosGet = await _spExecutor.QueryAsync(
                "sp_GetPedidoById",
                PedidoRepositoryHelper.MapToPedidoGetModel,
                new Dictionary<string, object> { { "@IdPedido", id } }
            );

            if (!pedidosGet.Any())
                return OperationResult.FailureResult("Pedido no encontrado");

            var pedido = PedidoRepositoryHelper.MapToPedido(pedidosGet.First());
            return OperationResult.SuccessResult("Pedido obtenido correctamente", pedido);
        },
        id
    );


        public Task<OperationResult> Save(Pedido entity) =>
     RepositoryLoggerHelper.ExecuteLoggedAsync<Pedido>(
         _logger,
         nameof(Save),
         async () =>
         {
             var validation = _pedidoValidator.ValidateForSave(entity);
             if (!validation.Success) return validation;

             var (parameters, outputParam) = PedidoRepositoryHelper.GetInsertParameters(entity);
             await _spExecutor.ExecuteAsync("sp_InsertPedido", parameters, outputParam);
             entity.IdPedido = (int)outputParam.Value;

             return OperationResult.SuccessResult("Pedido creado correctamente", entity);
         },
         entity.Cliente.IdUsuario
     );

        public Task<OperationResult> Update(Pedido entity) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Pedido>(
                _logger,
                nameof(Update),
                async () =>
                {
                    var validation = _pedidoValidator.ValidateForUpdate(entity);
                    if (!validation.Success) return validation;

                    var parameters = PedidoRepositoryHelper.GetUpdateParameters(entity);
                    await _spExecutor.ExecuteAsync("sp_UpdatePedido", parameters);

                    return OperationResult.SuccessResult("Pedido actualizado correctamente", entity);
                },
                entity.IdPedido
            );

        public Task<OperationResult> Remove(Pedido entity) =>
    RepositoryLoggerHelper.ExecuteLoggedAsync<Pedido>(
        _logger,
        nameof(Remove),
        async () =>
        {
            var validation = _pedidoValidator.ValidateForRemove(entity);
            if (!validation.Success) return validation;

            var parameters = PedidoRepositoryHelper.GetDeleteParameters(entity);
            await _spExecutor.ExecuteAsync("sp_DeletePedido", parameters);

            return OperationResult.SuccessResult("Pedido eliminado correctamente");
        },
        entity.IdPedido
    );


        public Task<OperationResult> GetAll(Expression<Func<Pedido, bool>> filter) =>
    RepositoryLoggerHelper.ExecuteLoggedAsync<Pedido>(
        _logger,
        nameof(GetAll),
        async () =>
        {
            var result = await GetAll();
            if (!result.Success || result.Data == null)
                return OperationResult.FailureResult("No se pudieron obtener pedidos");

            var pedidos = ((List<Pedido>)result.Data).AsQueryable().Where(filter.Compile()).ToList();
            return OperationResult.SuccessResult("Pedidos filtrados correctamente", pedidos);
        },
        filter
    );


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
                        PedidoRepositoryHelper.MapToPedidoGetModel,
                        parameters
                    );

                    var pedidos = pedidosGet.Select(PedidoRepositoryHelper.MapToPedido).ToList();
                    return OperationResult.SuccessResult("Pedidos obtenidos", pedidos);
                },
                clienteId
            );

            return result.Data as List<Pedido> ?? new List<Pedido>();
        }


    }
}
