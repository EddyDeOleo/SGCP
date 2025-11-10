
using SGCP.Application.Dtos.ModuloPedido.Pedido;
using SGCP.Domain.Entities.ModuloDePedido;

namespace SGCP.Application.Mappers
{
    public static class PedidoMapper
    {
        public static Pedido ToEntity(CreatePedidoDTO dto)
        {
            if (dto == null) return null;

            return new Pedido
            {
                ClienteId = dto.ClienteId,
                CarritoId = dto.CarritoId,
                Estado = "Pendiente",
                Total = 0,            
                FechaCreacion = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow
            };
        }

        public static PedidoGetDTO ToDto(Pedido entity)
        {
            if (entity == null) return null;

            return new PedidoGetDTO
            {
                IdPedido = entity.IdPedido,
                ClienteId = entity.ClienteId,
                CarritoId = entity.CarritoId,
                Total = entity.Total,
                Estado = entity.Estado,
                FechaCreacion = entity.FechaCreacion,
                FechaModificacion = entity.FechaModificacion,
                UsuarioModificacion = entity.UsuarioModificacion
            };
        }

        public static void MapToEntity(Pedido entity, UpdatePedidoDTO dto)
        {
            if (entity == null || dto == null) return;

            entity.ClienteId = dto.ClienteId;
            entity.CarritoId = dto.CarritoId;
            entity.Total = dto.Total;
            entity.Estado = dto.Estado;
            entity.FechaModificacion = DateTime.UtcNow;
        }
    }
}
