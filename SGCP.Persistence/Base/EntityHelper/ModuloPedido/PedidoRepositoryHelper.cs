using Microsoft.Data.SqlClient;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Persistence.Models.ModuloPedido.Pedido;
using System.Data;

namespace SGCP.Persistence.Base.EntityHelper.ModuloPedido
{
    public static class PedidoRepositoryHelper
    {
        public static PedidoGetModel MapToPedidoGetModel(SqlDataReader reader) =>
            new PedidoGetModel
            {
                IdPedido = reader.GetInt32(reader.GetOrdinal("pedido_id")),
                ClienteId = reader.GetInt32(reader.GetOrdinal("cliente_id")),
                CarritoId = reader.GetInt32(reader.GetOrdinal("carrito_id")),
                Total = reader.GetDecimal(reader.GetOrdinal("total")),
                Estado = reader.GetString(reader.GetOrdinal("estado")),
                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                FechaModificacion = reader.IsDBNull(reader.GetOrdinal("fecha_modificacion"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("fecha_modificacion")),
                UsuarioModificacion = reader.IsDBNull(reader.GetOrdinal("usuario_modificacion"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("usuario_modificacion"))
            };

        public static Pedido MapToPedido(PedidoGetModel model) =>
            new Pedido
            {
                IdPedido = model.IdPedido,
                ClienteId = model.ClienteId,
                CarritoId = model.CarritoId,
                Total = model.Total,
                Estado = model.Estado,
                FechaCreacion = model.FechaCreacion,
                FechaModificacion = model.FechaModificacion,
                UsuarioModificacion = model.UsuarioModificacion
            };

        public static (Dictionary<string, object> parameters, SqlParameter outputParam) GetInsertParameters(Pedido entity)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ClienteId", entity.ClienteId },
                { "@CarritoId", entity.CarritoId },
                { "@Total", entity.Total },
                { "@Estado", entity.Estado }
            };

            var outputParam = new SqlParameter("@IdPedido", SqlDbType.Int) { Direction = ParameterDirection.Output };
            return (parameters, outputParam);
        }

        public static Dictionary<string, object> GetUpdateParameters(Pedido entity) =>
            new Dictionary<string, object>
            {
                { "@IdPedido", entity.IdPedido },
                { "@ClienteId", entity.ClienteId },
                { "@CarritoId", entity.CarritoId },
                { "@Total", entity.Total },
                { "@Estado", entity.Estado },
                { "@UsuarioModificacion", entity.UsuarioModificacion ?? (object)DBNull.Value }
            };

        public static Dictionary<string, object> GetDeleteParameters(Pedido entity) =>
            new Dictionary<string, object>
            {
                { "@IdPedido", entity.IdPedido }
            };
    }
}
