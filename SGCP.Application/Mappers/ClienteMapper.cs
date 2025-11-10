

using SGCP.Application.Dtos.ModuloUsuarios.Cliente;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Mappers
{
    public static class ClienteMapper
    {
        public static void MapToEntity(Cliente cliente, UpdateClienteDTO dto, int? userId)
        {
            cliente.Nombre = dto.Nombre;
            cliente.Apellido = dto.Apellido;
            cliente.Username = dto.Username;
            cliente.Password = dto.Password;
            cliente.UsuarioModificacion = userId;
            cliente.FechaModificacion = DateTime.Now;
        }

        public static ClienteGetDTO ToDto(Cliente c)
        {
            return new ClienteGetDTO
            {
                ClienteId = c.IdUsuario,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Username = c.Username,
                FechaCreacion = c.FechaCreacion,
                FechaModificacion = c.FechaModificacion,
                UsuarioModificacion = c.UsuarioModificacion,
                Estatus = c.Estatus
            };
        }
        public static Cliente ToEntity(CreateClienteDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new Cliente(
                dto.Nombre,
                dto.Apellido,
                dto.Username,
                dto.Password
            )
            {
                Carrito = null,
                FechaCreacion = DateTime.Now,
                Estatus = true
            };
        }
    }
}
