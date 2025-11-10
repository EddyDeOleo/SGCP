

using Microsoft.Extensions.Logging;
using SGCP.Application.Dtos.ModuloUsuarios.Administrador;
using SGCP.Application.Interfaces.IServiceValidator.ModuloUsuarios;
using SGCP.Application.Repositories.ModuloUsuarios;

namespace SGCP.Application.Base.ServiceValidator.ModuloUsuarios
{
    public class AdminServiceValidator : ServiceValidator<AdminServiceValidator>, IAdminServiceValidator
    {
        private readonly IAdministrador _adminRepository;

        public AdminServiceValidator(
            ILogger<AdminServiceValidator> logger,
            IAdministrador adminRepository
        ) : base(logger)
        {
            _adminRepository = adminRepository;
        }

        public ServiceResult ValidateForCreate(CreateAdminDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de creación de administrador");
            if (!dtoVal.Success) return dtoVal;

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return Failure("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Apellido))
                return Failure("El apellido es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Username))
                return Failure("El username es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return Failure("La contraseña es obligatoria");

            return Success("DTO válido para crear administrador");
        }

        public ServiceResult ValidateForUpdate(UpdateAdminDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de actualización de administrador");
            if (!dtoVal.Success) return dtoVal;

            var idVal = ValidateId(dto.AdminId, "AdminId");
            if (!idVal.Success) return idVal;

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return Failure("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Apellido))
                return Failure("El apellido es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Username))
                return Failure("El username es obligatorio");

            return Success("DTO válido para actualizar administrador");
        }

        public ServiceResult ValidateForDelete(DeleteAdminDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de eliminación de administrador");
            if (!dtoVal.Success) return dtoVal;

            var idVal = ValidateId(dto.AdminId, "AdminId");
            if (!idVal.Success) return idVal;

            return Success("DTO válido para eliminar administrador");
        }

        public async Task<ServiceResult> ValidateAdminExistente(int adminId)
        {
            var idVal = ValidateId(adminId, "AdminId");
            if (!idVal.Success) return idVal;

            var result = await _adminRepository.GetEntityBy(adminId);
            if (!result.Success || result.Data == null)
                return Failure("Administrador no existe");

            return Success("Administrador existente", result.Data);
        }

        public async Task<ServiceResult> ValidateUsernameUnico(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Failure("Username no puede estar vacío");

            var allAdmins = await _adminRepository.GetAll();
            if (allAdmins.Success && allAdmins.Data is List<Domain.Entities.ModuloDeUsuarios.Administrador> admins)
            {
                if (admins.Any(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                    return Failure("El username ya está registrado");
            }

            return Success("Username válido y único");
        }
    }

}
