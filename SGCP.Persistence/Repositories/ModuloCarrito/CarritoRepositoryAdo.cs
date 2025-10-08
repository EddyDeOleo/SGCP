using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityHelper.ModuloCarrito;
using SGCP.Persistence.Base.EntityValidator.ModuloCarrito;
using System.Data;
using System.Linq.Expressions;


namespace SGCP.Persistence.Repositories.ModuloCarrito
{
    public class CarritoRepositoryAdo : ICarrito
    {

        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<CarritoRepositoryAdo> _logger;
        private readonly CarritoValidator _carritoValidator;


        public CarritoRepositoryAdo(IStoredProcedureExecutor spExecutor, ILogger<CarritoRepositoryAdo> logger, CarritoValidator carritoValidator)
        {
            _spExecutor = spExecutor;
            _logger = logger;
            _carritoValidator = carritoValidator;
        }



        public async Task<bool> Exists(Expression<Func<Carrito, bool>> filter)
        {
            var result = await RepositoryLoggerHelper.ExecuteLoggedAsync<Carrito>(
                _logger,
                nameof(Exists),
                async () =>
                {
                    var all = await GetAll();
                    if (!all.Success || all.Data == null)
                        return OperationResult.FailureResult("No se pudo obtener carritos para verificar existencia");

                    bool exists = ((List<Carrito>)all.Data).AsQueryable().Any(filter.Compile());
                    return OperationResult.SuccessResult("Existencia verificada", exists);
                },
                filter
            );

            return result.Success && result.Data is bool b && b;
        }

        public Task<OperationResult> GetAll() =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Carrito>(
                _logger,
                nameof(GetAll),
                async () =>
                {
                    var carritosGet = await _spExecutor.QueryAsync("sp_GetAllCarritos", CarritoRepositoryHelper.MapToCarritoGetModel);
                    var carritos = carritosGet.Select(CarritoRepositoryHelper.MapToCarrito).ToList();
                    return OperationResult.SuccessResult("Carritos obtenidos correctamente", carritos);
                }
            );

        public Task<OperationResult> GetEntityBy(int id) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Carrito>(
                _logger,
                nameof(GetEntityBy),
                async () =>
                {
                    var carritosGet = await _spExecutor.QueryAsync(
                        "sp_GetCarritoById",
                        CarritoRepositoryHelper.MapToCarritoGetModel,
                        new Dictionary<string, object> { { "@IdCarrito", id } }
                    );

                    if (!carritosGet.Any())
                        return OperationResult.FailureResult("Carrito no encontrado");

                    var carrito = CarritoRepositoryHelper.MapToCarrito(carritosGet.First());
                    return OperationResult.SuccessResult("Carrito obtenido correctamente", carrito);
                },
                id
            );

        public Task<OperationResult> Save(Carrito entity) =>
    RepositoryLoggerHelper.ExecuteLoggedAsync<Carrito>(
        _logger,
        nameof(Save),
        async () =>
        {
            var validation = _carritoValidator.ValidateForSave(entity);
            if (!validation.Success) return validation;

            var (parameters, outputParam) = CarritoRepositoryHelper.GetInsertParameters(entity);

            await _spExecutor.ExecuteAsync("sp_InsertCarrito", parameters, outputParam);

            entity.IdCarrito = (int)outputParam.Value;

            return OperationResult.SuccessResult("Carrito creado correctamente", entity);
        },
        entity.ClienteId
    );

        public Task<OperationResult> Update(Carrito entity) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Carrito>(
                _logger,
                nameof(Update),
                async () =>
                {
                    var validation = _carritoValidator.ValidateForUpdate(entity);
                    if (!validation.Success) return validation;

                    var parameters = CarritoRepositoryHelper.GetUpdateParameters(entity);
                    await _spExecutor.ExecuteAsync("sp_UpdateCarrito", parameters);

                    return OperationResult.SuccessResult("Carrito actualizado correctamente", entity);
                },
                entity.IdCarrito
            );

        public Task<OperationResult> Remove(Carrito entity) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Carrito>(
                _logger,
                nameof(Remove),
                async () =>
                {
                    var validation = _carritoValidator.ValidateForRemove(entity);
                    if (!validation.Success) return validation;

                    var parameters = CarritoRepositoryHelper.GetDeleteParameters(entity);
                    await _spExecutor.ExecuteAsync("sp_DeleteCarrito", parameters);

                    return OperationResult.SuccessResult("Carrito eliminado correctamente");
                },
                entity.IdCarrito
            );

        public Task<OperationResult> GetAll(Expression<Func<Carrito, bool>> filter) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Carrito>(
                _logger,
                nameof(GetAll),
                async () =>
                {
                    var all = await GetAll();
                    if (!all.Success || all.Data == null)
                        return OperationResult.FailureResult("No se pudieron obtener carritos");

                    var listaFiltrada = ((List<Carrito>)all.Data).AsQueryable().Where(filter.Compile()).ToList();
                    return OperationResult.SuccessResult("Carritos filtrados correctamente", listaFiltrada);
                },
                filter
            );

        public Task<Carrito?> GetByClienteId(int clienteId) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Carrito>(
                _logger,
                nameof(GetByClienteId),
                async () =>
                {
                    var result = await _spExecutor.QueryAsync(
                        "sp_GetCarritoByClienteId",
                        CarritoRepositoryHelper.MapToCarritoGetModel,
                        new Dictionary<string, object> { { "@ClienteId", clienteId } }
                    );

                    var model = result.FirstOrDefault();
                    if (model == null)
                        return OperationResult.FailureResult("No se encontró carrito para el cliente");

                    var carrito = CarritoRepositoryHelper.MapToCarrito(model);
                    return OperationResult.SuccessResult("Carrito obtenido", carrito);
                },
                clienteId
            ).ContinueWith(t => (Carrito?)t.Result.Data);
    }
}

