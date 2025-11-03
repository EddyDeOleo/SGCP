

namespace SGCP.Persistence.Models.ModuloReporte.Reporte
{
    public record ReporteGetModel
    {

        public int IdReporte { get; set; }
        public int AdminId { get; set; }
        public decimal TotalVentas { get; set; }
        public int TotalPedidos { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public bool Estatus { get; set; }
    }
}
