using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeProducto;

namespace SGCP.Domain.Entities.ModuloDeUsuarios
{
    public sealed class Cliente : Usuario
    {
        public Carrito Carrito { get; set; }
        public List<Pedido> HistorialPedidos { get; set; } = new();

        //
        /*
        public Carrito Carrito { get; set; }
        public List<Pedido> HistorialPedidos { get; set; }
        */


        public Cliente(int idUsuario, string nombre, string apellido, string username, string password)
            : base(idUsuario, nombre, apellido, username, password)
        {
            Carrito = new Carrito();
            HistorialPedidos = new List<Pedido>();
        }

        public Cliente(string nombre, string apellido, string username, string password)
    : base(nombre, apellido, username, password)
        {
            Carrito = new Carrito();
            HistorialPedidos = new List<Pedido>();
        }



        public void VerCatalogo(List<Producto> productos)
        {
            Console.WriteLine("Catálogo de productos:");
            foreach (var p in productos)
                Console.WriteLine($"ID: {p.IdProducto} | {p.Nombre} | ${p.Precio} | Stock: {p.Stock}");
        }

        public void AgregarProductoAlCarrito(Producto producto, int cantidad)
        {
            if (producto.VerificarDisponibilidad(cantidad))
            {
                Carrito.AgregarProducto(producto, cantidad);
                Console.WriteLine($"{cantidad} unidades de {producto.Nombre} agregadas al carrito.");
            }
            else
                Console.WriteLine($"No hay suficiente stock de {producto.Nombre}.");
        }

        public void RemoverProductoDelCarrito(Producto producto)
        {
            Carrito.QuitarProducto(producto);
            Console.WriteLine($"{producto.Nombre} removido del carrito.");
        }

        public Pedido FinalizarCompra()
        {
            if (Carrito.Productos.Count == 0)
            {
                Console.WriteLine("El carrito está vacío. No se puede finalizar la compra.");
                return null;
            }

            Pedido pedido = new Pedido(Carrito, this);
            pedido.ConfirmarPedido();
            HistorialPedidos.Add(pedido);
            Carrito.VaciarCarrito();
            Console.WriteLine($"Compra finalizada. Total: ${pedido.Total}");
            return pedido;
        }

        public static Cliente RegistrarUsuario(int idUsuario, string nombre, string apellido, string username, string password, List<Usuario> usuariosExistentes)
        {
            if (usuariosExistentes.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: El username ya está en uso.");
                return null;
            }

            var nuevoCliente = new Cliente(idUsuario, nombre, apellido, username, password);
            usuariosExistentes.Add(nuevoCliente);
            Console.WriteLine($"Cliente {username} registrado correctamente.");
            return nuevoCliente;
        }

    }
}
