

using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloReporte.Reporte
{
    public record UpdateReporteDTO : CreateReporteDTO
    {
        [Required(ErrorMessage = "El Id del reporte es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id del reporte debe ser válido.")]
        public int IdReporte { get; set; }
        [Required(ErrorMessage = "La fecha del reporte es obligatoria.")]
        public DateTime FechaCreacion { get; set; }

    }
}
