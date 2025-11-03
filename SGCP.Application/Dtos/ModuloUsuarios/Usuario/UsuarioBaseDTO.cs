
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloUsuarios.Usuario
{
    public abstract record UsuarioBaseDTO : BaseDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(50)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El username es obligatorio.")]
        [MaxLength(50)]
        public string Username { get; set; }

        


    }
}
