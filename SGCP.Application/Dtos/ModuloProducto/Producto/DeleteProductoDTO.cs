using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloProducto.Producto
{
    public record DeleteProductoDTO
    {
        [Required]
        public int IdProducto { get; set; }

    }
}
