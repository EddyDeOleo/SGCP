using SGCP.Application.Dtos.ModuloCarrito.Carrito;
using SGCP.Domain.Entities.ModuloDeCarrito;

namespace SGCP.Application.Mappers
{
    public static class CarritoMapper
    {
        public static Carrito ToEntity(CreateCarritoDTO dto)
        {
            return new Carrito
            {
                ClienteId = dto.ClienteId
            };
        }

        public static CarritoGetDTO ToDto(Carrito entity)
        {
            return new CarritoGetDTO
            {
                CarritoId = entity.IdCarrito,
                ClienteId = entity.ClienteId,
                FechaCreacion = entity.FechaCreacion,
                FechaModificacion = entity.FechaModificacion,
                UsuarioModificacion = entity.UsuarioModificacion,
                Estatus = entity.Estatus
            };
        }

        public static void MapToEntity(Carrito entity, UpdateCarritoDTO dto)
        {
            entity.ClienteId = dto.ClienteId;
            entity.FechaModificacion = DateTime.Now;
        }
    }
}
