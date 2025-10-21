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
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public Carrito Carrito { get; set; }
        public decimal Total { get;  set; }
        public string Estado { get; set; }

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
