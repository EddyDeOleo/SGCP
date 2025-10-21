using SGCP.Domain.Entities.ModuloDeProducto;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGCP.Persistence.Models.ModuloCarrito.Carrito
{
    public record CarritoGetModel
    {
        public int ClienteId { get; set; }

        //[Column("carrito_id")] 
        public int IdCarrito { get; set; }
        public List<Producto> Productos { get; private set; } = new List<Producto>();
        public Dictionary<Producto, int> Cantidades { get; private set; } = new Dictionary<Producto, int>();
    }
}
