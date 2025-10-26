
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloPedido.Pedido
{
    public record CreatePedidoDTO 
    {
        [Required(ErrorMessage = "El Id del cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El Id del carrito es obligatorio.")]
        public int CarritoId { get; set; }
    }
}
