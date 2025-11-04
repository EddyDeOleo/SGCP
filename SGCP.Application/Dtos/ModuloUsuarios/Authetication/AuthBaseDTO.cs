

using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloUsuarios.Authetication
{
    public abstract record AuthBaseDTO
    {
        [Required(ErrorMessage = "El username es obligatorio.")]
        [MaxLength(50)]
        public string Username { get; set; }

    }
}
