using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGCP.Application.Dtos.ModuloProducto.Producto
{
    public record UpdateProductoDTO : ProductoBaseDTO
    {
            [Required(ErrorMessage = "El Id del producto es obligatorio")]
            public int IdProducto { get; set; }
    }
}
