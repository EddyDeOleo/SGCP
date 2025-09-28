
namespace SGCP.Domain.Entities.ModuloDeProducto
{
    public sealed class Producto : Base.BaseEntity
    {
        public int IdProducto { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public decimal Precio { get; private set; }
        public int Stock { get; private set; }
        public string Categoria { get; private set; }

        public Producto(int idProducto, string nombre, string descripcion, decimal precio, int stock, string categoria)
        {
            IdProducto = idProducto;
            Nombre = nombre;
            Descripcion = descripcion;
            Precio = precio;
            Stock = stock;
            Categoria = categoria;
        }

        public bool VerificarDisponibilidad(int cantidad)
        {
            return Stock >= cantidad;
        }

        public void ActualizarStock(int cantidad)
        {
            if (cantidad <= Stock)
                Stock -= cantidad;
            else
                Console.WriteLine($"No hay suficiente stock de {Nombre}");
        }

        public void ActualizarProducto(string nombre, string descripcion, decimal precio, int stock, string categoria)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Precio = precio;
            Stock = stock;
            Categoria = categoria;
        }
    }
}
