using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGCP.Domain.Entities.ModuloDePedido
{
    public sealed class Pedido : Base.BaseEntity
    {
        [Key]
        [Column("pedido_id")]
        public int IdPedido { get;  set; }

        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Column("carrito_id")]
        public int CarritoId { get; set; }
        public Cliente Cliente { get; set; }
        public Carrito Carrito { get; set; }
        [Column("total")]
        public decimal Total { get;  set; }
        [Column("estado")]
        public string Estado { get; set; }

        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } 
        public Pedido(Carrito carrito, Cliente cliente)
        {
            Carrito = carrito;
            Cliente = cliente;
            Estado = "Pendiente";
        }

        public Pedido() { }

        public void ConfirmarPedido()
        {
            Estado = "Confirmado";
            Total = Carrito.CalcularTotal();
            foreach (var producto in Carrito.Productos)
                producto.ActualizarStock(Carrito.Cantidades[producto]);
        }

        public void CancelarPedido()
        {
            Estado = "Cancelado";
        }

        public void ActualizarEstado(string nuevoEstado)
        {
            Estado = nuevoEstado;
        }
    }
}
