using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Domain.Entities.ModuloDePedido
{
    public sealed class Pedido : Base.BaseEntity
    {
        public int IdPedido { get; private set; }
        public Cliente Cliente { get; private set; }
        public Carrito Carrito { get; private set; }
        public decimal Total { get; private set; }
        public string Estado { get; private set; }

        public Pedido(Carrito carrito, Cliente cliente)
        {
            Carrito = carrito;
            Cliente = cliente;
            Estado = "Pendiente";
        }

        public void ConfirmarPedido()
        {
            Estado = "Confirmado";
            Total = Carrito.CalcularTotal();
            foreach (var producto in Carrito.Productos)
                producto.ActualizarStock(Carrito.Cantidades[producto]);
        }

        public void CancelarPedido()
        {
            Estado = "Cancelado";
        }

        public void ActualizarEstado(string nuevoEstado)
        {
            Estado = nuevoEstado;
        }
    }
}
