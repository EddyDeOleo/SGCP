using Microsoft.Data.SqlClient;
using SGCP.Domain.Entities.ModuloDeReporte;
using SGCP.Persistence.Models.ModuloReporte.Reporte;

namespace SGCP.Persistence.Base.EntityHelper.ModuloReporte
{
    public static class ReporteRepositoryHelper
    {
        public static ReporteGetModel MapToReporteGetModel(SqlDataReader reader) => new ReporteGetModel
        {
            IdReporte = reader.GetInt32(reader.GetOrdinal("reporte_id")),
            AdminId = reader.GetInt32(reader.GetOrdinal("admin_id")),
            TotalVentas = reader.GetDecimal(reader.GetOrdinal("total_ventas")),
            TotalPedidos = reader.GetInt32(reader.GetOrdinal("total_pedidos")),
            FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
            FechaModificacion = reader.IsDBNull(reader.GetOrdinal("fecha_modificacion"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("fecha_modificacion")),
            UsuarioModificacion = reader.IsDBNull(reader.GetOrdinal("usuario_modificacion"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("usuario_modificacion")),
            Estatus = reader.GetBoolean(reader.GetOrdinal("estatus"))
        };

        public static Reporte MapToReporte(ReporteGetModel rgm) => new Reporte
        {
            IdReporte = rgm.IdReporte,
            AdminId = rgm.AdminId,
            TotalVentas = rgm.TotalVentas,
            TotalPedidos = rgm.TotalPedidos,
            FechaCreacion = rgm.FechaCreacion,
            FechaModificacion = rgm.FechaModificacion,
            UsuarioModificacion = rgm.UsuarioModificacion,
            Estatus = rgm.Estatus
        };

        public static (Dictionary<string, object> Parameters, SqlParameter OutputParam) GetInsertParameters(Reporte entity)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AdminId", entity.AdminId },
                { "@TotalVentas", entity.TotalVentas },
                { "@TotalPedidos", entity.TotalPedidos }
            };

            var outputParam = new SqlParameter("@IdReporte", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            return (parameters, outputParam);
        }

        public static Dictionary<string, object> GetUpdateParameters(Reporte entity) => new Dictionary<string, object>
        {
            { "@IdReporte", entity.IdReporte },
            { "@AdminId", entity.AdminId },
            { "@TotalVentas", entity.TotalVentas },
            { "@TotalPedidos", entity.TotalPedidos },
            { "@UsuarioModificacion", entity.UsuarioModificacion ?? (object)DBNull.Value }
        };

        public static Dictionary<string, object> GetDeleteParameters(Reporte entity) => new Dictionary<string, object>
        {
            { "@IdReporte", entity.IdReporte }
        };
    }
}
