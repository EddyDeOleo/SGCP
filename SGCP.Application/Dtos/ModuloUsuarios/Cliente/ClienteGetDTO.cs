
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloUsuarios.Cliente
{
    public record ClienteGetDTO : ClienteBaseDTO
    {
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MaxLength(255)]
        public string Password { get; set; }
    }
}
