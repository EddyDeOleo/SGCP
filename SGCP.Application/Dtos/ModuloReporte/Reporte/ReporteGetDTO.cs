
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloReporte.Reporte
{
    public record ReporteGetDTO : ReporteBaseDTO
    {
        [Required(ErrorMessage = "El Id del reporte es obligatorio.")]
        public int IdReporte { get; set; }
    }
}
