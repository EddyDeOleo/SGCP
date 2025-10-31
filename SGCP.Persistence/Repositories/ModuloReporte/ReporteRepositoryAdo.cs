using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloReporte;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeReporte;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityHelper.ModuloReporte;
using SGCP.Persistence.Base.EntityValidator.ModuloReporte;


namespace SGCP.Persistence.Repositories.ModuloReporte
{
    public class ReporteRepositoryAdo : BaseRepositoryAdo<Reporte>, IReporte
    {
        protected override string SpGetAll => "sp_GetAllReportes";
        protected override string SpGetById => "sp_GetReporteById";
        protected override string SpInsert => "sp_InsertReporte";
        protected override string SpUpdate => "sp_UpdateReporte";
        protected override string SpDelete => "sp_DeleteReporte";

        public ReporteRepositoryAdo(
            IStoredProcedureExecutor spExecutor,
            ILogger<ReporteRepositoryAdo> logger,
            ReporteValidator reporteValidator)
            : base(spExecutor, logger, reporteValidator)
        {
        }

        protected override Reporte MapToEntity(SqlDataReader reader)
        {
            var model = ReporteRepositoryHelper.MapToReporteGetModel(reader);
            return ReporteRepositoryHelper.MapToReporte(model);
        }


        protected override (Dictionary<string, object>, SqlParameter) GetInsertParameters(Reporte entity)
        {
            return ReporteRepositoryHelper.GetInsertParameters(entity);
        }

        protected override Dictionary<string, object> GetUpdateParameters(Reporte entity)
        {
            return ReporteRepositoryHelper.GetUpdateParameters(entity);
        }

        protected override Dictionary<string, object> GetDeleteParameters(Reporte entity)
        {
            return ReporteRepositoryHelper.GetDeleteParameters(entity);
        }

        protected override Dictionary<string, object> GetIdParameter(int id)
        {
            return new Dictionary<string, object> { { "@IdReporte", id } };
        }

        public async Task<List<Reporte>> GetByAdministradorId(int adminId)
        {
            var result = await RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
                _logger,
                nameof(GetByAdministradorId),
                async () =>
                {
                    var parameters = new Dictionary<string, object> { { "@AdministradorId", adminId } };

                    var reportesGet = await _spExecutor.QueryAsync(
                        "sp_GetReportesByAdministrador",
                        MapToEntity,
                        parameters
                    );

                    var reportes = reportesGet.ToList();
                    return OperationResult.SuccessResult("Reportes obtenidos correctamente", reportes);
                },
                adminId
            );

            return result.Data as List<Reporte> ?? new List<Reporte>();
        }

        public async Task<List<Reporte>> GetByFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            var result = await RepositoryLoggerHelper.ExecuteLoggedAsync<Reporte>(
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
                        MapToEntity,
                        parameters
                    );

                    var reportes = reportesGet.ToList();
                    return OperationResult.SuccessResult("Reportes obtenidos correctamente", reportes);
                },
                new { fechaInicio, fechaFin }
            );

            return result.Data as List<Reporte> ?? new List<Reporte>();
        }
    }
}

