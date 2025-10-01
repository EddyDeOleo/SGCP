using SGCP.Domain.Entities.ModuloDeProducto;

namespace SGCP.Domain.Entities.ModuloDeCarrito
{
    public sealed class Carrito : Base.BaseEntity   

    {
        public int ClienteId { get; set; } 

        public int IdCarrito { get; set; }
        public List<Producto> Productos { get; set; } = new List<Producto>();
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
