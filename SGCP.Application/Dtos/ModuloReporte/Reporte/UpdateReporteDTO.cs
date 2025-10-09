

using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloReporte.Reporte
{
    public record UpdateReporteDTO : ReporteBaseDTO
    {
        [Required(ErrorMessage = "El Id del reporte es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id del reporte debe ser válido.")]
        public int IdReporte { get; set; }

    }
}
