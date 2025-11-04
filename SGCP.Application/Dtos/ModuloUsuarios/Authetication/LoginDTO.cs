


using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloUsuarios.Authetication
{
    public record class LoginDTO : AuthBaseDTO
    {

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MaxLength(255)]
        public string Password { get; set; }

    }
}
