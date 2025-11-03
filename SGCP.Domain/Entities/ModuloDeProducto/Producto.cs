
using System.ComponentModel.DataAnnotations;

namespace SGCP.Domain.Entities.ModuloDeProducto
{
    public sealed class Producto : Base.BaseEntity
    {
        [Key]
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; }

      public Producto(){}
        
           
        public Producto(int idProducto, string nombre, string descripcion, decimal precio, int stock, string categoria)
        {
            IdProducto = idProducto;
            Nombre = nombre;
            Descripcion = descripcion;
            Precio = precio;
            Stock = stock;
            Categoria = categoria;
        }

    }
}
