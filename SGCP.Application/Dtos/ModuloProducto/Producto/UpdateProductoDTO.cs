
using System.ComponentModel.DataAnnotations;


namespace SGCP.Application.Dtos.ModuloProducto.Producto
{
    public record UpdateProductoDTO : ProductoBaseDTO
    {
            [Required(ErrorMessage = "El Id del producto es obligatorio")]
            public int IdProducto { get; set; }
    }
}
