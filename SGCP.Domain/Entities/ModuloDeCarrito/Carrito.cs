using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGCP.Domain.Entities.ModuloDeCarrito
{
    public sealed class Carrito : Base.BaseEntity   

    {
        public int ClienteId { get; set; }

        [Key]
        [Column("carrito_id")] 
        public int IdCarrito { get; set; }
        public List<Producto> Productos { get; set; } = new List<Producto>();

        //
        public Cliente Cliente { get; set; }

        [NotMapped]
        public Dictionary<Producto, int> Cantidades { get;  set; } = new Dictionary<Producto, int>();

        public Carrito() { }

        public void AgregarProducto(Producto producto, int cantidad)
        {
            if (Cantidades.ContainsKey(producto))
                Cantidades[producto] += cantidad;
            else
                Cantidades[producto] = cantidad;

            if (!Productos.Contains(producto))
                Productos.Add(producto);
        }

        public void QuitarProducto(Producto producto)
        {
            if (Cantidades.ContainsKey(producto))
            {
                Cantidades.Remove(producto);
                Productos.Remove(producto);
            }
        }

        public decimal CalcularTotal()
        {
            decimal total = 0;
            foreach (var producto in Productos)
                total += producto.Precio * Cantidades[producto];
            return total;
        }

        public void VaciarCarrito()
        {
            Productos.Clear();
            Cantidades.Clear();
        }
    }
}
