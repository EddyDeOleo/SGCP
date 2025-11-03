
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloPedido.Pedido
{
    public record PedidoGetDTO : UpdatePedidoDTO
    {
        [Required(ErrorMessage = "La fecha del pedido es obligatoria.")]
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacion { get; set; }


    }
}
