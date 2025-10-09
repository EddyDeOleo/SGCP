
using System.ComponentModel.DataAnnotations;

namespace SGCP.Application.Dtos.ModuloProducto.Producto
{
    public abstract record ProductoBaseDTO
    {
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no debe exceder X caracteres")]
        public string Nombre { get; set; }

        [MaxLength(255, ErrorMessage = "La descripción no debe exceder X caracteres")]
        public string? Descripcion { get; set; }

        [MaxLength(50, ErrorMessage = "La categoría no debe exceder X caracteres")]
        public string? Categoria { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }
    }
}
