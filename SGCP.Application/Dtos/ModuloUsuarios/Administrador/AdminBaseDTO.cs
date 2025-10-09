
using SGCP.Application.Dtos.ModuloUsuarios.Usuario;
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloUsuarios.Administrador
{
    public record AdminBaseDTO : UsuarioBaseDTO
    {
        [Required(ErrorMessage = "El Id del admin es obligatorio.")]
        public int AdminId { get; set; }
    }
}
