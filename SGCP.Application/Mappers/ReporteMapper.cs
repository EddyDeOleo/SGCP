using SGCP.Application.Dtos.ModuloReporte.Reporte;
using SGCP.Domain.Entities.ModuloDeReporte;


namespace SGCP.Application.Mappers
{
    public static class ReporteMapper
    {
        public static Reporte ToEntity(CreateReporteDTO dto, int adminId)
        {
            return new Reporte
            {
                AdminId = adminId,
                TotalVentas = dto.TotalVentas,
                TotalPedidos = dto.TotalPedidos
            };
        }

        public static ReporteGetDTO ToDto(Reporte entity)
        {
            return new ReporteGetDTO
            {
                IdReporte = entity.IdReporte,
                AdminId = entity.AdminId,
                TotalVentas = entity.TotalVentas,
                TotalPedidos = entity.TotalPedidos,
                FechaCreacion = entity.FechaCreacion,
                FechaModificacion = entity.FechaModificacion,
                UsuarioModificacion = entity.UsuarioModificacion,
                Estatus = entity.Estatus
            };
        }

        public static void MapToEntity(Reporte entity, UpdateReporteDTO dto, int usuarioModificacion)
        {
            entity.AdminId = dto.AdminId;
            entity.TotalVentas = dto.TotalVentas;
            entity.TotalPedidos = dto.TotalPedidos;
            entity.FechaCreacion = dto.FechaCreacion;
            entity.FechaModificacion = DateTime.Now;
            entity.UsuarioModificacion = usuarioModificacion;
        }
    }
}
