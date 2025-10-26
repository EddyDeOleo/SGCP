

namespace SGCP.Application.Dtos.ModuloCarrito.CarritoProducto
{
    public record CarritoProductoGetDTO : AgregarProductoDTO
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }

    }
}
