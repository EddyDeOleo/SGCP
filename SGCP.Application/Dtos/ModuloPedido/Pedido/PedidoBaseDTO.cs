
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloPedido.Pedido
{
    public abstract record PedidoBaseDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El total no puede estar vacío.")]
        [Range(0, double.MaxValue, ErrorMessage = "El total del pedido no puede ser negativo.")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "El estado del pedido es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El estado del pedido no puede exceder los X caracteres.")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "Debe asociar un carrito al pedido.")]
        public int? CarritoId { get; set; }

  
    }
}
