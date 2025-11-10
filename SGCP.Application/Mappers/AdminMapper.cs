using SGCP.Application.Dtos.ModuloUsuarios.Administrador;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Mappers
{


    public static class AdminMapper
    {
        public static AdminGetDTO ToDto(Administrador admin)
        {
            return new AdminGetDTO
            {
                AdminId = admin.IdUsuario,
                Nombre = admin.Nombre,
                Apellido = admin.Apellido,
                Username = admin.Username,
                FechaCreacion = admin.FechaCreacion,
                FechaModificacion = admin.FechaModificacion,
                UsuarioModificacion = admin.UsuarioModificacion,
                Estatus = admin.Estatus
            };
        }

        public static void MapToEntity(Administrador admin, UpdateAdminDTO dto, int? userId)
        {
            admin.Nombre = dto.Nombre;
            admin.Apellido = dto.Apellido;
            admin.Username = dto.Username;
            admin.Password = dto.Password;
            admin.UsuarioModificacion = userId;
            admin.FechaModificacion = DateTime.UtcNow;
        }
    }
}

    


