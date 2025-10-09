using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloUsuarios.Cliente
{
    public record DeleteClienteDTO
    {
        [Required(ErrorMessage = "El Id del cliente es obligatorio.")]
        public int ClienteId { get; set; }

    }
}
