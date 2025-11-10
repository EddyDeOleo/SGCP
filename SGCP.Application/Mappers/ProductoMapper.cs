

using SGCP.Application.Dtos.ModuloProducto.Producto;
using SGCP.Domain.Entities.ModuloDeProducto;

namespace SGCP.Application.Mappers
{
    public static class ProductoMapper
    {
        // Mapear CreateProductoDTO → Producto
        public static Producto ToEntity(CreateProductoDTO dto)
        {
            return new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Categoria = dto.Categoria,
                Precio = dto.Precio,
                Stock = dto.Stock,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };
        }

        // Mapear Producto → ProductoGetDTO
        public static ProductoGetDTO ToDto(Producto producto)
        {
            return new ProductoGetDTO
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Categoria = producto.Categoria,
                Precio = producto.Precio,
                Stock = producto.Stock,
                FechaCreacion = producto.FechaCreacion,
                FechaModificacion = producto.FechaModificacion,
                UsuarioModificacion = producto.UsuarioModificacion
            };
        }

        // Mapear UpdateProductoDTO → Producto existente
        public static void MapToEntity(Producto producto, UpdateProductoDTO dto)
        {
            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Categoria = dto.Categoria;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock;
            producto.FechaModificacion = DateTime.Now;
        }
    }
}
