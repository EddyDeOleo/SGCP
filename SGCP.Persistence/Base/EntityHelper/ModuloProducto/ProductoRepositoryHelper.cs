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
            Stock = reader.GetInt32(reader.GetOrdinal("stock"))
        };

        public static Producto MapToProducto(ProductoGetModel model) => new Producto(
            model.IdProducto,
            model.Nombre,
            model.Descripcion,
            model.Precio,
            model.Stock,
            model.Categoria
        );

        public static (Dictionary<string, object> parameters, SqlParameter outputParam) GetInsertParameters(Producto entity)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Nombre", entity.Nombre },
                { "@Descripcion", entity.Descripcion },
                { "@Categoria", entity.Categoria },
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
                { "@Descripcion", entity.Descripcion },
                { "@Categoria", entity.Categoria },
                { "@Precio", entity.Precio },
                { "@Stock", entity.Stock }
            };

        public static Dictionary<string, object> GetDeleteParameters(Producto entity) =>
            new Dictionary<string, object> { { "@IdProducto", entity.IdProducto } };
    }
}


