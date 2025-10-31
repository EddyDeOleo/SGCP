using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Administrador;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Services
{
    public sealed class AdminService : IAdminService
    {
        private readonly IAdministrador _repository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdministrador repository, ILogger<AdminService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ServiceResult> CreateAdmin(CreateAdminDTO createAdminDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation("Creando un nuevo administrador");

            try
            {
                // CU-07: No debe existir un admin con el mismo username
                var existingResult = await _repository.GetAll();
                if (existingResult.Success && existingResult.Data != null)
                {
                    if (((List<Administrador>)existingResult.Data)
                        .Any(a => a.Username.Equals(createAdminDto.Username, StringComparison.OrdinalIgnoreCase)))
                    {
                        result.Success = false;
                        result.Message = "El username ya está registrado";
                        return result;
                    }
                }

                var admin = new Administrador(
                    createAdminDto.Nombre,
                    createAdminDto.Apellido,
                    createAdminDto.Username,
                    createAdminDto.Password
                );

                var opResult = await _repository.Save(admin);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = opResult.Message;
                    return result;
                }

                result.Success = true;
                result.Message = "Administrador creado correctamente";
                result.Data = admin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear administrador");
                result.Success = false;
                result.Message = "Ocurrió un error al crear el administrador";
            }

            return result;
        }

        public async Task<ServiceResult> GetAdmin()
        {
            var result = new ServiceResult();
            _logger.LogInformation("Obteniendo todos los administradores");

            try
            {
               

                var opResult = await _repository.GetAll();
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "No se pudieron obtener los administradores";
                    return result;
                }

                var adminsDto = ((List<Administrador>)opResult.Data).Select(a => new AdminGetDTO
                {
                    AdminId = a.IdUsuario,
                    Nombre = a.Nombre,
                    Apellido = a.Apellido,
                    Username = a.Username,
                    Password = a.Password
                }).ToList();

                result.Success = true;
                result.Message = "Administradores obtenidos correctamente";
                result.Data = adminsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener administradores");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener los administradores";
            }

            return result;
        }

        public async Task<ServiceResult> GetAdminById(int id)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Obteniendo administrador con ID {id}");

            try
            {
               
                var opResult = await _repository.GetEntityBy(id);
                if (!opResult.Success || opResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Administrador no encontrado";
                    return result;
                }

                var admin = (Administrador)opResult.Data;

                var adminDto = new AdminGetDTO
                {
                    AdminId = admin.IdUsuario,
                    Nombre = admin.Nombre,
                    Apellido = admin.Apellido,
                    Username = admin.Username,
                    Password = admin.Password
                };

                result.Success = true;
                result.Message = "Administrador obtenido correctamente";
                result.Data = adminDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener administrador con ID {id}");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener el administrador";
            }

            return result;
        }

        public async Task<ServiceResult> UpdateAdmin(UpdateAdminDTO updateAdminDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Actualizando administrador con ID {updateAdminDto.AdminId}");

            try
            {
             

                var existingResult = await _repository.GetEntityBy(updateAdminDto.AdminId);
                if (!existingResult.Success || existingResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Administrador no encontrado";
                    return result;
                }

                var admin = (Administrador)existingResult.Data;

                admin.Nombre = updateAdminDto.Nombre;
                admin.Apellido = updateAdminDto.Apellido;
                admin.Username = updateAdminDto.Username;
                admin.Password = updateAdminDto.Password;

                var opResult = await _repository.Update(admin);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = opResult.Message;
                    return result;
                }

                result.Success = true;
                result.Message = "Administrador actualizado correctamente";
                result.Data = admin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar administrador con ID {updateAdminDto.AdminId}");
                result.Success = false;
                result.Message = "Ocurrió un error al actualizar el administrador";
            }

            return result;
        }

        public async Task<ServiceResult> RemoveAdmin(DeleteAdminDTO deleteAdminDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation($"Eliminando administrador con ID {deleteAdminDto.AdminId}");

            try
            {
              
                var existingResult = await _repository.GetEntityBy(deleteAdminDto.AdminId);
                if (!existingResult.Success || existingResult.Data == null)
                {
                    result.Success = false;
                    result.Message = "Administrador no encontrado";
                    return result;
                }

                var admin = (Administrador)existingResult.Data;
                var opResult = await _repository.Remove(admin);

                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = opResult.Message;
                    return result;
                }

                result.Success = true;
                result.Message = "Administrador eliminado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar administrador con ID {deleteAdminDto.AdminId}");
                result.Success = false;
                result.Message = "Ocurrió un error al eliminar el administrador";
            }

            return result;
        }
    }
}
