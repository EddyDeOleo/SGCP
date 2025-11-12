using Microsoft.Data.SqlClient;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Persistence.Models.ModuloCarrito.Carrito;
using System.Data;

namespace SGCP.Persistence.Base.EntityHelper.ModuloCarrito
{
    public static class CarritoRepositoryHelper
    {
        public static CarritoGetModel MapToCarritoGetModel(IDataReader reader)
        {
            return new CarritoGetModel
            {
                IdCarrito = reader.GetInt32(reader.GetOrdinal("carrito_id")),
                ClienteId = reader.GetInt32(reader.GetOrdinal("cliente_id")),
                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                FechaModificacion = reader.IsDBNull(reader.GetOrdinal("fecha_modificacion"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("fecha_modificacion")),
                UsuarioModificacion = reader.IsDBNull(reader.GetOrdinal("usuario_modificacion"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("usuario_modificacion")),
                Estatus = reader.GetBoolean(reader.GetOrdinal("estatus"))
            };
        }

        public static Carrito MapToCarrito(CarritoGetModel model)
        {
            return new Carrito
            {
                IdCarrito = model.IdCarrito,
                ClienteId = model.ClienteId,
                FechaCreacion = model.FechaCreacion,
                FechaModificacion = model.FechaModificacion,
                UsuarioModificacion = model.UsuarioModificacion,
                Estatus = model.Estatus,
                Productos = new List<Producto>(),
                Cantidades = new Dictionary<Producto, int>()
            };
        }

        public static (Dictionary<string, object> parameters, SqlParameter outputParam) GetInsertParameters(Carrito entity)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ClienteId", entity.ClienteId }
            };

            var outputParam = new SqlParameter("@IdCarrito", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            return (parameters, outputParam);
        }

        public static Dictionary<string, object> GetUpdateParameters(Carrito entity)
        {
            return new Dictionary<string, object>
            {
                { "@IdCarrito", entity.IdCarrito },
                { "@ClienteId", entity.ClienteId },
                { "@UsuarioModificacion", entity.UsuarioModificacion ?? (object)DBNull.Value }
            };
        }

        public static Dictionary<string, object> GetDeleteParameters(Carrito entity)
        {
            return new Dictionary<string, object>
            {
                { "@IdCarrito", entity.IdCarrito }
            };
        }
    }
}
