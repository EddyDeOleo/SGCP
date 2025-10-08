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
            TotalVentas = reader.GetDecimal(reader.GetOrdinal("total_ventas")),
            TotalPedidos = reader.GetInt32(reader.GetOrdinal("total_pedidos"))
        };

        public static Reporte MapToReporte(ReporteGetModel rgm) => new Reporte
        {
            IdReporte = rgm.IdReporte,
            TotalVentas = rgm.TotalVentas,
            TotalPedidos = rgm.TotalPedidos
        };

        public static (Dictionary<string, object> Parameters, SqlParameter OutputParam) GetInsertParameters(Reporte entity)
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

            return (parameters, outputParam);
        }

        public static Dictionary<string, object> GetUpdateParameters(Reporte entity) => new Dictionary<string, object>
    {
        { "@IdReporte", entity.IdReporte },
        { "@AdminId", entity.AdminId },
        { "@Fecha", entity.FechaCreacion },
        { "@TotalVentas", entity.TotalVentas },
        { "@TotalPedidos", entity.TotalPedidos }
    };

        public static Dictionary<string, object> GetDeleteParameters(Reporte entity) => new Dictionary<string, object>
    {
        { "@IdReporte", entity.IdReporte }
    };
    }
}
