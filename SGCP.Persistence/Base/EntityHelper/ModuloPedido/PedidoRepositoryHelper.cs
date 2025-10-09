using Microsoft.Data.SqlClient;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeUsuarios;
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
                Cliente = new Cliente(reader.GetInt32(reader.GetOrdinal("cliente_id")), "", "", "", ""),
                Carrito = new Carrito { IdCarrito = reader.GetInt32(reader.GetOrdinal("carrito_id")) },
                Total = reader.GetDecimal(reader.GetOrdinal("total")),
                Estado = reader.GetString(reader.GetOrdinal("estado"))
            };

        public static Pedido MapToPedido(PedidoGetModel model) =>
            new Pedido(model.Carrito, model.Cliente)
            {
                IdPedido = model.IdPedido,
                Total = model.Total,
                Estado = model.Estado
            };

        public static (Dictionary<string, object> parameters, SqlParameter outputParam) GetInsertParameters(Pedido entity)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ClienteId", entity.Cliente.IdUsuario },
                { "@CarritoId", entity.Carrito.IdCarrito },
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
                { "@ClienteId", entity.Cliente.IdUsuario },
                { "@CarritoId", entity.Carrito.IdCarrito },
                { "@Total", entity.Total },
                { "@Estado", entity.Estado }
            };

        public static Dictionary<string, object> GetDeleteParameters(Pedido entity) =>
      new Dictionary<string, object>
      {
        { "@IdPedido", entity.IdPedido }
      };

    }
}
