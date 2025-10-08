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
                ClienteId = reader.GetInt32(reader.GetOrdinal("cliente_id"))
            };
        }

        public static Carrito MapToCarrito(CarritoGetModel model)
        {
            return new Carrito
            {
                IdCarrito = model.IdCarrito,
                ClienteId = model.ClienteId,
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
                { "@ClienteId", entity.ClienteId }
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
