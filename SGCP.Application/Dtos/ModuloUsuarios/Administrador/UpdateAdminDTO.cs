
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloUsuarios.Administrador
{
    public record UpdateAdminDTO : CreateAdminDTO
    {
        [Required(ErrorMessage = "El Id del admin es obligatorio.")]
        public int AdminId { get; set; }
    }
}
