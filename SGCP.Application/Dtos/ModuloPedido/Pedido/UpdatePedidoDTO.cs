
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloPedido.Pedido
{
    public record UpdatePedidoDTO : PedidoBaseDTO
    {
        [Required(ErrorMessage = "El Id del pedido es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id del pedido debe ser válido para actualizar.")]
        public int IdPedido { get; set; }
    }
}
