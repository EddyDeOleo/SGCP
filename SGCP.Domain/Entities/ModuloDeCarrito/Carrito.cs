using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGCP.Domain.Entities.ModuloDeCarrito
{
    public sealed class Carrito : Base.BaseEntity   

    {
        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Key]
        [Column("carrito_id")] 
        public int IdCarrito { get; set; }
        [NotMapped]
        public List<Producto> Productos { get; set; } = new List<Producto>();
        [NotMapped]
        public List<CarritoProducto> CarritoProductos { get; set; } = new List<CarritoProducto>();


        public Cliente Cliente { get; set; }

        [NotMapped]
        public Dictionary<Producto, int> Cantidades { get;  set; } = new Dictionary<Producto, int>();

        public Carrito() { }

       
    }
}
