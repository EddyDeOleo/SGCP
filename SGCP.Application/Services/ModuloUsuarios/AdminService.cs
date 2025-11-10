using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Base.ServiceValidator.ModuloUsuarios;
using SGCP.Application.Dtos.ModuloUsuarios.Administrador;
using SGCP.Application.Interfaces;
using SGCP.Application.Mappers;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Services.ModuloUsuarios
{
    public sealed class AdminService : BaseService<AdminService>, IAdminService
    {
        private readonly IAdministrador _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly AdminServiceValidator _adminServiceValidator;

        public AdminService(
            IAdministrador repository,
            ILogger<AdminService> logger,
            ICurrentUserService currentUserService,
            AdminServiceValidator adminServiceValidator
        ) : base(logger)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _adminServiceValidator = adminServiceValidator;
        }

        public async Task<ServiceResult> CreateAdmin(CreateAdminDTO dto)
        {
            return await ExecuteSafeAsync("crear el administrador", async () =>
            {
                var validation = _adminServiceValidator.ValidateForCreate(dto);
                if (!validation.Success) return validation;

                var usernameVal = await _adminServiceValidator.ValidateUsernameUnico(dto.Username);
                if (!usernameVal.Success) return usernameVal;

                var admin = new Administrador(dto.Nombre, dto.Apellido, dto.Username, dto.Password);
                var opResult = await _repository.Save(admin);

                if (!opResult.Success)
                    return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Administrador creado correctamente", admin);
            });
        }

        public async Task<ServiceResult> GetAdmin()
        {
            return await ExecuteSafeAsync("obtener los administradores", async () =>
            {
                var opResult = await _repository.GetAll();
                if (!opResult.Success || opResult.Data == null)
                    return new ServiceResult(false, "No se pudieron obtener los administradores");

                var adminsDto = ((List<Administrador>)opResult.Data)
                    .Select(AdminMapper.ToDto)
                    .ToList();

                return new ServiceResult(true, "Administradores obtenidos correctamente", adminsDto);
            });
        }

        public async Task<ServiceResult> GetAdminById(int id)
        {
            return await ExecuteSafeAsync("obtener el administrador", async () =>
            {
                var existVal = await _adminServiceValidator.ValidateAdminExistente(id);
                if (!existVal.Success) return existVal;

                var admin = (Administrador)existVal.Data;
                return new ServiceResult(true, "Administrador obtenido correctamente", AdminMapper.ToDto(admin));
            });
        }

        public async Task<ServiceResult> UpdateAdmin(UpdateAdminDTO dto)
        {
            return await ExecuteSafeAsync("actualizar el administrador", async () =>
            {
                var validation = _adminServiceValidator.ValidateForUpdate(dto);
                if (!validation.Success) return validation;

                var existVal = await _adminServiceValidator.ValidateAdminExistente(dto.AdminId);
                if (!existVal.Success) return existVal;

                var admin = (Administrador)existVal.Data;
                AdminMapper.MapToEntity(admin, dto, _currentUserService.GetUserId());

                var opResult = await _repository.Update(admin);
                if (!opResult.Success)
                    return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Administrador actualizado correctamente", admin);
            });
        }

        public async Task<ServiceResult> RemoveAdmin(DeleteAdminDTO dto)
        {
            return await ExecuteSafeAsync("eliminar el administrador", async () =>
            {
                var validation = _adminServiceValidator.ValidateForDelete(dto);
                if (!validation.Success) return validation;

                var existVal = await _adminServiceValidator.ValidateAdminExistente(dto.AdminId);
                if (!existVal.Success) return existVal;

                var admin = (Administrador)existVal.Data;
                var opResult = await _repository.Remove(admin);

                if (!opResult.Success)
                    return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Administrador eliminado correctamente");
            });
        }
    }
}


