
namespace SGCP.Application.Dtos.ModuloProducto.Producto
{
    public record ProductoGetDTO : UpdateProductoDTO
    {
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
