using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Domain.Entities.ModuloDeCarrito;

namespace SGCP.Persistence.Models.ModuloPedido.Pedido
{
     public record PedidoGetModel
    {
        public int IdPedido { get; set; }
        public Cliente Cliente { get; set; }
        public Carrito Carrito { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

    }
}
