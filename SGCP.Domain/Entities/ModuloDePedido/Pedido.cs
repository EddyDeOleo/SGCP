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
        public int? CarritoId { get; set; }
        public Cliente Cliente { get; set; }
        public Carrito Carrito { get; set; }
        [Column("total")]
        public decimal Total { get;  set; }
        [Column("estado")]
        public string Estado { get; set; }

     
        public Pedido(Carrito carrito, Cliente cliente)
        {
            Carrito = carrito;
            Cliente = cliente;
            Estado = "Pendiente";
        }

        public Pedido() { }

        

    }
}
