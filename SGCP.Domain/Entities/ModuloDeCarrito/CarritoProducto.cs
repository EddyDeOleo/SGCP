

using SGCP.Domain.Entities.ModuloDeProducto;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGCP.Domain.Entities.ModuloDeCarrito
{
    public sealed class CarritoProducto
    {
        [Column("carrito_id")]
        public int CarritoId { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        public Carrito Carrito { get; set; }
        public Producto Producto { get; set; }
    }
}
