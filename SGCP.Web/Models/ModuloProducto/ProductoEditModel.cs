using System.ComponentModel.DataAnnotations;

namespace SGCP.Web.Models.ModuloProducto
{
    public class ProductoEditModel
    {
        [Required(ErrorMessage = "El Id del producto es obligatorio")]
        public int idProducto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no debe exceder X caracteres")]
        public string nombre { get; set; }

        [MaxLength(255, ErrorMessage = "La descripción no debe exceder X caracteres")]
        public string? descripcion { get; set; }

        [MaxLength(50, ErrorMessage = "La categoría no debe exceder X caracteres")]
        public string? categoria { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero")]
        public decimal precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int stock { get; set; }
    }


    public class Response_EP_Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}
