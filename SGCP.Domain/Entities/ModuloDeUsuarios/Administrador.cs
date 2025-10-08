using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Domain.Entities.ModuloDeReporte;

namespace SGCP.Domain.Entities.ModuloDeUsuarios
{
    public sealed class Administrador : Usuario
    {
        public Administrador(int idUsuario, string nombre, string apellido, string username, string password)
           : base(idUsuario, nombre, apellido, username, password) { }

        public void AgregarProducto(List<Producto> inventario, Producto producto)
        {
            inventario.Add(producto);
            Console.WriteLine($"Producto {producto.Nombre} agregado al inventario.");
        }

        public void EditarProducto(List<Producto> inventario, int productoId, Producto nuevoProducto)
        {
            var prod = inventario.FirstOrDefault(p => p.IdProducto == productoId);
            if (prod != null)
            {
                prod.ActualizarProducto(
                    nuevoProducto.Nombre,
                    nuevoProducto.Descripcion,
                    nuevoProducto.Precio,
                    nuevoProducto.Stock,
                    nuevoProducto.Categoria
                );
                Console.WriteLine($"Producto {productoId} actualizado.");
            }
            else
                Console.WriteLine("Producto no encontrado.");
        }

        public void EliminarProducto(List<Producto> inventario, int productoId)
        {
            var prod = inventario.FirstOrDefault(p => p.IdProducto == productoId);
            if (prod != null)
            {
                inventario.Remove(prod);
                Console.WriteLine($"Producto {productoId} eliminado del inventario.");
            }
            else
                Console.WriteLine("Producto no encontrado.");
        }

        public void GestionarPedidos(List<Pedido> pedidos)
        {
            Console.WriteLine("Pedidos en gestión:");
            foreach (var pedido in pedidos)
                Console.WriteLine($"Pedido {pedido.IdPedido} | Cliente: {pedido.Cliente.Nombre} | Estado: {pedido.Estado}");
        }

        public Reporte GenerarReporte(List<Pedido> ventas)
        {
            Reporte reporte = new Reporte();
            reporte.GenerarReporte(ventas);
            Console.WriteLine($"Reporte generado: {reporte.TotalPedidos} pedidos, total ventas ${reporte.TotalVentas}");
            return reporte;
        }

        public static Administrador RegistrarUsuario(int idUsuario, string nombre, string apellido, string username, string password, List<Usuario> usuariosExistentes)
        {
            if (usuariosExistentes.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: El username ya está en uso.");
                return null;
            }

            var nuevoAdmin = new Administrador(idUsuario, nombre, apellido, username, password);
            usuariosExistentes.Add(nuevoAdmin);
            Console.WriteLine($"Administrador {username} registrado correctamente.");
            return nuevoAdmin;
        }
    }
}
