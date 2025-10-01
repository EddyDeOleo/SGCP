using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloReporte;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeReporte;
using SGCP.Persistence.Base;
using SGCP.Persistence.Models.ModuloReporte.Reporte;
using System.Linq.Expressions;

namespace SGCP.Persistence.Repositories.ModuloReporte
{
    public class ReporteRepositoryAdo : IReporte
    {

        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<ReporteRepositoryAdo> _logger;

        public ReporteRepositoryAdo(IStoredProcedureExecutor spExecutor, ILogger<ReporteRepositoryAdo> logger)
        {
            _spExecutor = spExecutor;
            _logger = logger;
        }


        public async Task<bool> Exists(Expression<Func<Reporte, bool>> filter)
        {
            _logger.LogInformation("Verificando existencia de reportes con filtro {Filter}", filter);
            try
            {
                var reportesGet = await _spExecutor.QueryAsync(
                    "sp_GetAllReportes",
                    reader => new ReporteGetModel
                    {
                        IdReporte = reader.GetInt32(reader.GetOrdinal("reporte_id")),
                        TotalVentas = reader.GetDecimal(reader.GetOrdinal("total_ventas")),
                        TotalPedidos = reader.GetInt32(reader.GetOrdinal("total_pedidos"))
                    }
                );

                var reportes = reportesGet.Select(rgm => new Reporte
                {
                    IdReporte = rgm.IdReporte,
                    TotalVentas = rgm.TotalVentas,
                    TotalPedidos = rgm.TotalPedidos
                }).AsQueryable();

                bool exists = reportes.Any(filter.Compile());
                _logger.LogInformation("Existencia verificada: {Exists}", exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de reportes");
                return false;
            }
        }

        public async Task<OperationResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los reportes");
            try
            {
                var reportesGet = await _spExecutor.QueryAsync(
                    "sp_GetAllReportes",
                    reader => new ReporteGetModel
                    {
                        IdReporte = reader.GetInt32(reader.GetOrdinal("reporte_id")),
                        TotalVentas = reader.GetDecimal(reader.GetOrdinal("total_ventas")),
                        TotalPedidos = reader.GetInt32(reader.GetOrdinal("total_pedidos"))
                    }
                );

                var reportes = reportesGet.Select(rgm => new Reporte
                {
                    IdReporte = rgm.IdReporte,
                    TotalVentas = rgm.TotalVentas,
                    TotalPedidos = rgm.TotalPedidos
                }).ToList();

                _logger.LogInformation("{Count} reportes obtenidos", reportes.Count);
                return OperationResult.SuccessResult("Reportes obtenidos correctamente", reportes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reportes");
                return OperationResult.FailureResult($"Error al obtener reportes: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetAll(Expression<Func<Reporte, bool>> filter)
        {
            _logger.LogInformation("Obteniendo reportes filtrados");
            try
            {
                var reportesGet = await _spExecutor.QueryAsync(
                    "sp_GetAllReportes",
                    reader => new ReporteGetModel
                    {
                        IdReporte = reader.GetInt32(reader.GetOrdinal("reporte_id")),
                        TotalVentas = reader.GetDecimal(reader.GetOrdinal("total_ventas")),
                        TotalPedidos = reader.GetInt32(reader.GetOrdinal("total_pedidos"))
                    }
                );

                var reportes = reportesGet.Select(rgm => new Reporte
                {
                    IdReporte = rgm.IdReporte,
                    TotalVentas = rgm.TotalVentas,
                    TotalPedidos = rgm.TotalPedidos
                }).AsQueryable();

                var filtered = reportes.Where(filter.Compile()).ToList();
                _logger.LogInformation("{Count} reportes filtrados obtenidos", filtered.Count);
                return OperationResult.SuccessResult("Reportes filtrados correctamente", filtered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar reportes");
                return OperationResult.FailureResult($"Error al filtrar reportes: {ex.Message}");
            }
        }

        public async Task<List<Reporte>> GetByAdministradorId(int adminId)
        {
            _logger.LogInformation("Obteniendo reportes del administrador Id {AdminId}", adminId);
            try
            {
                var parameters = new Dictionary<string, object> { { "@AdministradorId", adminId } };

                var reportesGet = await _spExecutor.QueryAsync(
                    "sp_GetReportesByAdministrador",
                    reader => new ReporteGetModel
                    {
                        IdReporte = reader.GetInt32(reader.GetOrdinal("reporte_id")),
                        TotalVentas = reader.GetDecimal(reader.GetOrdinal("total_ventas")),
                        TotalPedidos = reader.GetInt32(reader.GetOrdinal("total_pedidos"))
                    },
                    parameters
                );

                var reportes = reportesGet.Select(rgm => new Reporte
                {
                    IdReporte = rgm.IdReporte,
                    TotalVentas = rgm.TotalVentas,
                    TotalPedidos = rgm.TotalPedidos
                }).ToList();

                _logger.LogInformation("{Count} reportes obtenidos para el administrador Id {AdminId}", reportes.Count, adminId);
                return reportes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reportes para el administrador Id {AdminId}", adminId);
                throw;
            }
        }

        public async Task<List<Reporte>> GetByFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            _logger.LogInformation("Obteniendo reportes entre {FechaInicio} y {FechaFin}", fechaInicio, fechaFin);
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@FechaInicio", fechaInicio },
                    { "@FechaFin", fechaFin }
                };

                var reportesGet = await _spExecutor.QueryAsync(
                    "sp_GetReportesByFecha",
                    reader => new ReporteGetModel
                    {
                        IdReporte = reader.GetInt32(reader.GetOrdinal("reporte_id")),
                        TotalVentas = reader.GetDecimal(reader.GetOrdinal("total_ventas")),
                        TotalPedidos = reader.GetInt32(reader.GetOrdinal("total_pedidos"))
                    },
                    parameters
                );

                var reportes = reportesGet.Select(rgm => new Reporte
                {
                    IdReporte = rgm.IdReporte,
                    TotalVentas = rgm.TotalVentas,
                    TotalPedidos = rgm.TotalPedidos
                }).ToList();

                _logger.LogInformation("{Count} reportes obtenidos entre las fechas indicadas", reportes.Count);
                return reportes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reportes entre fechas {FechaInicio} - {FechaFin}", fechaInicio, fechaFin);
                throw;
            }
        }

        public async Task<OperationResult> GetEntityBy(int id)
        {
            _logger.LogInformation("Obteniendo reporte con Id {Id}", id);
            try
            {
                var reportesGet = await _spExecutor.QueryAsync(
                    "sp_GetReporteById",
                    reader => new ReporteGetModel
                    {
                        IdReporte = reader.GetInt32(reader.GetOrdinal("reporte_id")),
                        TotalVentas = reader.GetDecimal(reader.GetOrdinal("total_ventas")),
                        TotalPedidos = reader.GetInt32(reader.GetOrdinal("total_pedidos"))
                    },
                    new Dictionary<string, object> { { "@IdReporte", id } }
                );

                if (!reportesGet.Any())
                {
                    _logger.LogWarning("Reporte con Id {Id} no encontrado", id);
                    return OperationResult.FailureResult("Reporte no encontrado");
                }

                var rgm = reportesGet.First();
                var reporte = new Reporte
                {
                    IdReporte = rgm.IdReporte,
                    TotalVentas = rgm.TotalVentas,
                    TotalPedidos = rgm.TotalPedidos
                };

                _logger.LogInformation("Reporte con Id {Id} obtenido correctamente", id);
                return OperationResult.SuccessResult("Reporte obtenido correctamente", reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte con Id {Id}", id);
                return OperationResult.FailureResult($"Error al obtener reporte: {ex.Message}");
            }
        }

        public async Task<OperationResult> Remove(Reporte entity)
        {
            _logger.LogInformation("Eliminando reporte Id {IdReporte}", entity?.IdReporte);
            if (entity == null)
            {
                _logger.LogWarning("Reporte nulo no puede ser eliminado");
                return OperationResult.FailureResult("El reporte no puede ser nulo.");
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@IdReporte", entity.IdReporte }
                };

                await _spExecutor.ExecuteAsync("sp_DeleteReporte", parameters);
                _logger.LogInformation("Reporte eliminado correctamente Id {IdReporte}", entity.IdReporte);
                return OperationResult.SuccessResult("Reporte eliminado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar reporte Id {IdReporte}", entity.IdReporte);
                return OperationResult.FailureResult($"Error al eliminar reporte: {ex.Message}");
            }
        }

        public async Task<OperationResult> Save(Reporte entity)
        {
            _logger.LogInformation("Creando reporte");
            if (entity == null)
            {
                _logger.LogWarning("Reporte nulo no puede ser creado");
                return OperationResult.FailureResult("El reporte no puede ser nulo.");
            }

            if (entity.TotalVentas < 0 || entity.TotalPedidos < 0)
            {
                _logger.LogWarning("Valores inválidos de ventas o pedidos para el reporte");
                return OperationResult.FailureResult("El total de ventas o pedidos no puede ser negativo.");
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@AdminId", entity.AdminId },
                    { "@Fecha", entity.FechaCreacion },
                    { "@TotalVentas", entity.TotalVentas },
                    { "@TotalPedidos", entity.TotalPedidos }
                };

                var outputParam = new SqlParameter("@IdReporte", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                await _spExecutor.ExecuteAsync("sp_InsertReporte", parameters, outputParam);
                entity.IdReporte = (int)outputParam.Value;

                _logger.LogInformation("Reporte creado correctamente Id {IdReporte}", entity.IdReporte);
                return OperationResult.SuccessResult("Reporte creado correctamente", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear reporte");
                return OperationResult.FailureResult($"Error al crear reporte: {ex.Message}");
            }
        }

        public async Task<OperationResult> Update(Reporte entity)
        {
            _logger.LogInformation("Actualizando reporte Id {IdReporte}", entity?.IdReporte);
            if (entity == null)
            {
                _logger.LogWarning("Reporte nulo no puede ser actualizado");
                return OperationResult.FailureResult("El reporte no puede ser nulo.");
            }

            if (entity.TotalVentas < 0 || entity.TotalPedidos < 0)
            {
                _logger.LogWarning("Valores inválidos de ventas o pedidos para actualizar reporte Id {IdReporte}", entity.IdReporte);
                return OperationResult.FailureResult("El total de ventas o pedidos no puede ser negativo.");
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@IdReporte", entity.IdReporte },
                    { "@AdminId", entity.AdminId },
                    { "@Fecha", entity.FechaCreacion },
                    { "@TotalVentas", entity.TotalVentas },
                    { "@TotalPedidos", entity.TotalPedidos }
                };

                await _spExecutor.ExecuteAsync("sp_UpdateReporte", parameters);
                _logger.LogInformation("Reporte actualizado correctamente Id {IdReporte}", entity.IdReporte);
                return OperationResult.SuccessResult("Reporte actualizado correctamente", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar reporte Id {IdReporte}", entity.IdReporte);
                return OperationResult.FailureResult($"Error al actualizar reporte: {ex.Message}");
            }
        }
    }
}
