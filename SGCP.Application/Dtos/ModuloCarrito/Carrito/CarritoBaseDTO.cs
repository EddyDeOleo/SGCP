
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloCarrito.Carrito
{
    public abstract record CarritoBaseDTO
    {
        [Required(ErrorMessage = "El Id del cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El Id del carrito es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id del carrito debe ser válido.")]
        public int CarritoId { get; set; }
    }
}
