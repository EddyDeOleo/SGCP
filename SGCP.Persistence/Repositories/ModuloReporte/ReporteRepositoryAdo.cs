using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloReporte;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeReporte;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityHelper.ModuloReporte;
using SGCP.Persistence.Base.EntityValidator.ModuloReporte;
using System.Linq.Expressions;

namespace SGCP.Persistence.Repositories.ModuloReporte
{
    public class ReporteRepositoryAdo : IReporte
    {

        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<ReporteRepositoryAdo> _logger;
        private readonly ReporteValidator _reporteValidator;

        public ReporteRepositoryAdo(IStoredProcedureExecutor spExecutor, ILogger<ReporteRepositoryAdo> logger, ReporteValidator reporteValidator)
        {
            _spExecutor = spExecutor;
            _logger = logger;
            _reporteValidator = reporteValidator;
        }
        public async Task<bool> Exists(Expression<Func<Reporte, bool>> filter)
        {
            var result = await RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
                _logger,
                nameof(Exists),
                async () =>
                {
                    var all = await GetAll();
                    if (!all.Success || all.Data == null)
                        return OperationResult.FailureResult("No se pudo obtener reportes para verificar existencia");

                    bool exists = ((List<Reporte>)all.Data).AsQueryable().Any(filter.Compile());
                    return OperationResult.SuccessResult("Existencia verificada", exists);
                },
                filter
            );

            return result.Success && result.Data is bool b && b;
        }

        public Task<OperationResult> GetAll() =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
                _logger,
                nameof(GetAll),
                async () =>
                {
                    var reportesGet = await _spExecutor.QueryAsync("sp_GetAllReportes", ReporteRepositoryHelper.MapToReporteGetModel);
                    var reportes = reportesGet.Select(ReporteRepositoryHelper.MapToReporte).ToList();
                    return OperationResult.SuccessResult("Reportes obtenidos correctamente", reportes);
                }
            );

        public Task<OperationResult> GetEntityBy(int id) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
                _logger,
                nameof(GetEntityBy),
                async () =>
                {
                    var reportesGet = await _spExecutor.QueryAsync(
                        "sp_GetReporteById",
                        ReporteRepositoryHelper.MapToReporteGetModel,
                        new Dictionary<string, object> { { "@IdReporte", id } }
                    );

                    if (!reportesGet.Any())
                        return OperationResult.FailureResult("Reporte no encontrado");

                    var reporte = ReporteRepositoryHelper.MapToReporte(reportesGet.First());
                    return OperationResult.SuccessResult("Reporte obtenido correctamente", reporte);
                },
                id
            );

        public Task<OperationResult> Save(Reporte entity) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
                _logger,
                nameof(Save),
                async () =>
                {
                    var validation = _reporteValidator.ValidateForSave(entity);
                    if (!validation.Success) return validation;

                    var (parameters, outputParam) = ReporteRepositoryHelper.GetInsertParameters(entity);
                    await _spExecutor.ExecuteAsync("sp_InsertReporte", parameters, outputParam);
                    entity.IdReporte = (int)outputParam.Value;

                    return OperationResult.SuccessResult("Reporte creado correctamente", entity);
                },
                entity.AdminId
            );

        public Task<OperationResult> Update(Reporte entity) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
                _logger,
                nameof(Update),
                async () =>
                {
                    var validation = _reporteValidator.ValidateForUpdate(entity);
                    if (!validation.Success) return validation;

                    var parameters = ReporteRepositoryHelper.GetUpdateParameters(entity);
                    await _spExecutor.ExecuteAsync("sp_UpdateReporte", parameters);

                    return OperationResult.SuccessResult("Reporte actualizado correctamente", entity);
                },
                entity.IdReporte
            );

        public Task<OperationResult> Remove(Reporte entity) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
                _logger,
                nameof(Remove),
                async () =>
                {
                    var validation = _reporteValidator.ValidateForRemove(entity);
                    if (!validation.Success) return validation;

                    var parameters = ReporteRepositoryHelper.GetDeleteParameters(entity);
                    await _spExecutor.ExecuteAsync("sp_DeleteReporte", parameters);

                    return OperationResult.SuccessResult("Reporte eliminado correctamente");
                },
                entity.IdReporte
            );

        public Task<OperationResult> GetAll(Expression<Func<Reporte, bool>> filter) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
                _logger,
                nameof(GetAll),
                async () =>
                {
                    var all = await GetAll();
                    if (!all.Success || all.Data == null)
                        return OperationResult.FailureResult("No se pudieron obtener reportes");

                    var listaFiltrada = ((List<Reporte>)all.Data).AsQueryable().Where(filter.Compile()).ToList();
                    return OperationResult.SuccessResult("Reportes filtrados correctamente", listaFiltrada);
                },
                filter
            );

        public Task<List<Reporte>> GetByAdministradorId(int adminId) =>
            RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
                _logger,
                nameof(GetByAdministradorId),
                async () =>
                {
                    var parameters = new Dictionary<string, object> { { "@AdministradorId", adminId } };
                    var reportesGet = await _spExecutor.QueryAsync("sp_GetReportesByAdministrador", ReporteRepositoryHelper.MapToReporteGetModel, parameters);
                    var reportes = reportesGet.Select(ReporteRepositoryHelper.MapToReporte).ToList();
                    return OperationResult.SuccessResult("Reportes obtenidos correctamente", reportes);
                },
                adminId
            ).ContinueWith(t => (List<Reporte>)t.Result.Data!);

        public Task<List<Reporte>> GetByFecha(DateTime fechaInicio, DateTime fechaFin) =>
     RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
         _logger,
         nameof(GetByFecha),
         async () =>
         {
             var parameters = new Dictionary<string, object>
             {
                { "@FechaInicio", fechaInicio },
                { "@FechaFin", fechaFin }
             };
             var reportesGet = await _spExecutor.QueryAsync(
                 "sp_GetReportesByFecha",
                 ReporteRepositoryHelper.MapToReporteGetModel,
                 parameters
             );
             var reportes = reportesGet.Select(ReporteRepositoryHelper.MapToReporte).ToList();
             return OperationResult.SuccessResult("Reportes obtenidos correctamente", reportes);
         },
         new object[] { fechaInicio, fechaFin } 
     ).ContinueWith(t => (List<Reporte>)t.Result.Data!);
    }
}

