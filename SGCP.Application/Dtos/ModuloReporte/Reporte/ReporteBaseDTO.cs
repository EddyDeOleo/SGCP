
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloReporte.Reporte
{
    public abstract record ReporteBaseDTO
    {
        [Required(ErrorMessage = "El Id del administrador es obligatorio.")]
        public int AdminId { get; set; }

        [Required(ErrorMessage = "La fecha del reporte es obligatoria.")]
        public DateTime FechaCreacion { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El total de ventas no puede ser negativo.")]
        public decimal TotalVentas { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El total de pedidos no puede ser negativo.")]
        public int TotalPedidos { get; set; }
    }

  
}
