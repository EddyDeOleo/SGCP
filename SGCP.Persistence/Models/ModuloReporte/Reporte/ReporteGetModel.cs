

namespace SGCP.Persistence.Models.ModuloReporte.Reporte
{
    public record ReporteGetModel
    {

        public int IdReporte { get; set; }
        public decimal TotalVentas { get; set; }
        public int TotalPedidos { get; set; }

    }
}
