

using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloCarrito.Carrito
{
    public record CreateCarritoDTO
    {
        [Required(ErrorMessage = "El Id del cliente es obligatorio.")]
        public int ClienteId { get; set; }
    }
}
