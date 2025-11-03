using Microsoft.Data.SqlClient;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Persistence.Models.ModuloProducto.Producto;
using System.Data;


namespace SGCP.Persistence.Base.EntityHelper.ModuloProducto
{
    public static class ProductoRepositoryHelper
    {
        public static ProductoGetModel MapToProductoGetModel(SqlDataReader reader) => new ProductoGetModel
        {
            IdProducto = reader.GetInt32(reader.GetOrdinal("producto_id")),
            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
            Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "" : reader.GetString(reader.GetOrdinal("descripcion")),
            Categoria = reader.IsDBNull(reader.GetOrdinal("categoria")) ? "" : reader.GetString(reader.GetOrdinal("categoria")),
            Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
            Stock = reader.GetInt32(reader.GetOrdinal("stock")),

            FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
            FechaModificacion = reader.IsDBNull(reader.GetOrdinal("fecha_modificacion"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("fecha_modificacion")),
            UsuarioModificacion = reader.IsDBNull(reader.GetOrdinal("usuario_modificacion"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("usuario_modificacion")),
            Estatus = reader.GetBoolean(reader.GetOrdinal("estatus"))
        };

        public static Producto MapToProducto(ProductoGetModel model)
        {
            var producto = new Producto(
                model.IdProducto,
                model.Nombre,
                model.Descripcion,
                model.Precio,
                model.Stock,
                model.Categoria
            );

            producto.FechaCreacion = model.FechaCreacion;
            producto.FechaModificacion = model.FechaModificacion;
            producto.UsuarioModificacion = model.UsuarioModificacion;
            producto.Estatus = model.Estatus;

            return producto;
        }

        public static (Dictionary<string, object> parameters, SqlParameter outputParam) GetInsertParameters(Producto entity)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Nombre", entity.Nombre },
                { "@Descripcion", entity.Descripcion ?? (object)DBNull.Value },
                { "@Categoria", entity.Categoria ?? (object)DBNull.Value },
                { "@Precio", entity.Precio },
                { "@Stock", entity.Stock }
            };

            var outputParam = new SqlParameter("@IdProducto", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            return (parameters, outputParam);
        }

        public static Dictionary<string, object> GetUpdateParameters(Producto entity) =>
            new Dictionary<string, object>
            {
                { "@IdProducto", entity.IdProducto },
                { "@Nombre", entity.Nombre },
                { "@Descripcion", entity.Descripcion ?? (object)DBNull.Value },
                { "@Categoria", entity.Categoria ?? (object)DBNull.Value },
                { "@Precio", entity.Precio },
                { "@Stock", entity.Stock },
                
                { "@UsuarioModificacion", entity.UsuarioModificacion ?? (object)DBNull.Value }
            };

        public static Dictionary<string, object> GetDeleteParameters(Producto entity) =>
            new Dictionary<string, object>
            {
                { "@IdProducto", entity.IdProducto },
                { "@UsuarioModificacion", entity.UsuarioModificacion ?? (object)DBNull.Value }
            };
    }
}


