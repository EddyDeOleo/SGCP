

namespace SGCP.Application.Dtos.ModuloCarrito.CarritoProducto
{
    public record AgregarProductoDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
