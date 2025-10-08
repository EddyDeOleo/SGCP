
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloPedido.Pedido
{
    public record PedidoBaseDTO
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El total del pedido no puede ser negativo.")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "El estado del pedido es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El estado del pedido no puede exceder los X caracteres.")]
        public string Estado { get; set; }

        public int? CarritoId { get; set; }
    }
}
