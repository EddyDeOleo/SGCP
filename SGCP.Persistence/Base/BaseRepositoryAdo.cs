
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Domain.Repository;
using SGCP.Persistence.Base.EntityValidator;
using System.Data;
using System.Linq.Expressions;

namespace SGCP.Persistence.Base
{
    public abstract class BaseRepositoryAdo<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly IStoredProcedureExecutor _spExecutor;
        protected readonly ILogger _logger;
        protected readonly EntityValidator<TEntity>? _validator;

        protected abstract string SpGetAll { get; }
        protected abstract string SpGetById { get; }
        protected abstract string SpInsert { get; }
        protected abstract string SpUpdate { get; }
        protected abstract string SpDelete { get; }

        protected abstract TEntity MapToEntity(SqlDataReader reader);
        protected abstract (Dictionary<string, object> parameters, SqlParameter outputParam) GetInsertParameters(TEntity entity);
        protected abstract Dictionary<string, object> GetUpdateParameters(TEntity entity);
        protected abstract Dictionary<string, object> GetDeleteParameters(TEntity entity);

        public BaseRepositoryAdo(IStoredProcedureExecutor spExecutor, ILogger logger, EntityValidator<TEntity>? validator = null)
        {
            _spExecutor = spExecutor;
            _logger = logger;
            _validator = validator;
        }

        public virtual async Task<OperationResult> GetAll()
        {
            return await RepositoryLoggerHelper.ExecuteLoggedAsync<TEntity>(
                _logger,
                nameof(GetAll),
                async () =>
                {
                    var results = await _spExecutor.QueryAsync(SpGetAll, MapToEntity);
                    var entities = results.ToList();
                    return OperationResult.SuccessResult($"{typeof(TEntity).Name}s obtenidos correctamente", entities);
                }
            );
        }

        public virtual async Task<OperationResult> GetAll(Expression<Func<TEntity, bool>> filter)
        {
            return await RepositoryLoggerHelper.ExecuteLoggedAsync<TEntity>(
                _logger,
                nameof(GetAll),
                async () =>
                {
                    var all = await GetAll();
                    if (!all.Success || all.Data == null)
                        return OperationResult.FailureResult($"No se pudieron obtener {typeof(TEntity).Name}s");

                    var listaFiltrada = ((List<TEntity>)all.Data).AsQueryable().Where(filter.Compile()).ToList();
                    return OperationResult.SuccessResult($"{typeof(TEntity).Name}s filtrados correctamente", listaFiltrada);
                },
                filter
            );
        }

        public virtual async Task<OperationResult> GetEntityBy(int id)
        {
            return await RepositoryLoggerHelper.ExecuteLoggedAsync<TEntity>(
                _logger,
                nameof(GetEntityBy),
                async () =>
                {
                    var results = await _spExecutor.QueryAsync(
                        SpGetById,
                        MapToEntity,
                        GetIdParameter(id)
                    );

                    if (!results.Any())
                        return OperationResult.FailureResult($"{typeof(TEntity).Name} no encontrado");

                    var entity = results.First();
                    return OperationResult.SuccessResult($"{typeof(TEntity).Name} obtenido correctamente", entity);
                },
                id
            );
        }

        public virtual async Task<OperationResult> Save(TEntity entity)
        {
            return await RepositoryLoggerHelper.ExecuteLoggedAsync<TEntity>(
                _logger,
                nameof(Save),
                async () =>
                {
                    if (_validator != null)
                    {
                        var validation = _validator.ValidateForSave(entity);
                        if (!validation.Success)
                            return validation;
                    }
                    else if (entity == null)
                    {
                        return OperationResult.FailureResult($"El objeto {typeof(TEntity).Name} no puede ser nulo");
                    }

                    var (parameters, outputParam) = GetInsertParameters(entity);

                    await _spExecutor.ExecuteAsync(SpInsert, parameters, outputParam);

                    return OperationResult.SuccessResult($"{typeof(TEntity).Name} creado correctamente", entity);
                },
                entity
            );
        }

        public virtual async Task<OperationResult> Update(TEntity entity)
        {
            return await RepositoryLoggerHelper.ExecuteLoggedAsync<TEntity>(
                _logger,
                nameof(Update),
                async () =>
                {
                    if (_validator != null)
                    {
                        var validation = _validator.ValidateForUpdate(entity);
                        if (!validation.Success)
                            return validation;
                    }
                    else if (entity == null)
                    {
                        return OperationResult.FailureResult($"El objeto {typeof(TEntity).Name} no puede ser nulo");
                    }

                    var parameters = GetUpdateParameters(entity);
                    await _spExecutor.ExecuteAsync(SpUpdate, parameters);

                    return OperationResult.SuccessResult($"{typeof(TEntity).Name} actualizado correctamente", entity);
                },
                entity
            );
        }

        public virtual async Task<OperationResult> Remove(TEntity entity)
        {
            return await RepositoryLoggerHelper.ExecuteLoggedAsync<TEntity>(
                _logger,
                nameof(Remove),
                async () =>
                {
                    if (_validator != null)
                    {
                        var validation = _validator.ValidateForRemove(entity);
                        if (!validation.Success)
                            return validation;
                    }
                    else if (entity == null)
                    {
                        return OperationResult.FailureResult($"El objeto {typeof(TEntity).Name} no puede ser nulo");
                    }

                    var parameters = GetDeleteParameters(entity);
                    await _spExecutor.ExecuteAsync(SpDelete, parameters);

                    return OperationResult.SuccessResult($"{typeof(TEntity).Name} eliminado correctamente");
                },
                entity
            );
        }

        public virtual async Task<bool> Exists(Expression<Func<TEntity, bool>> filter)
        {
            var result = await RepositoryLoggerHelper.ExecuteLoggedAsync<TEntity>(
                _logger,
                nameof(Exists),
                async () =>
                {
                    var all = await GetAll();
                    if (!all.Success || all.Data == null)
                        return OperationResult.FailureResult("No se pudo obtener registros para verificar existencia");

                    bool exists = ((List<TEntity>)all.Data).AsQueryable().Any(filter.Compile());
                    return OperationResult.SuccessResult("Existencia verificada", exists);
                },
                filter
            );

            return result.Success && result.Data is bool b && b;
        }

        protected virtual Dictionary<string, object> GetIdParameter(int id)
        {
            return new Dictionary<string, object> { { "@Id", id } };
        }
    }
}
