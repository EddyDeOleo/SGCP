
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloCarrito.Carrito
{
    public record DeleteCarritoDTO
    {

        [Required(ErrorMessage = "El Id del carrito es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id del carrito debe ser válido.")]
        public int CarritoId { get; set; }
    }
}
