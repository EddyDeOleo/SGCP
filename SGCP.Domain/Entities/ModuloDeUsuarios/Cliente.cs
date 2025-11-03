using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeProducto;

namespace SGCP.Domain.Entities.ModuloDeUsuarios
{
    public sealed class Cliente : Usuario
    {
        public Carrito Carrito { get; set; }
        public List<Pedido> HistorialPedidos { get; set; } = new();

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



    }
}
