using SGCP.Application.Dtos.ModuloUsuarios.Usuario;
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloUsuarios.Cliente
{
    public record ClienteBaseDTO : UsuarioBaseDTO
    {
        [Required(ErrorMessage = "El Id del cliente es obligatorio.")]
        public int ClienteId { get; set; }
    }
}
