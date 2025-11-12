

using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloUsuarios.Administrador
{
    public record AdminGetDTO : AdminBaseDTO
    {

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MaxLength(255)]
        public string Password { get; set; }
    }
}
