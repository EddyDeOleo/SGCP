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

        public Cliente() { }
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
