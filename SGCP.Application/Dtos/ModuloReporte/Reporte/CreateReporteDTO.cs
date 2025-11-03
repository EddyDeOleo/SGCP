

using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloReporte.Reporte
{
    public record CreateReporteDTO 
    {
        [Required(ErrorMessage = "El Id del administrador es obligatorio.")]
        public int AdminId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El total de ventas no puede ser negativo.")]
        public decimal TotalVentas { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El total de pedidos no puede ser negativo.")]
        public int TotalPedidos { get; set; }

    }
}
