using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Administrador;

namespace SGCP.Application.Interfaces.IServiceValidator.ModuloUsuarios
{
    public interface IAdminServiceValidator : IServiceValidatorBase<CreateAdminDTO, UpdateAdminDTO, DeleteAdminDTO>
    {
        Task<ServiceResult> ValidateAdminExistente(int adminId);
        Task<ServiceResult> ValidateUsernameUnico(string username);
    }
}
