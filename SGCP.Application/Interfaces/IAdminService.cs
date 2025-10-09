
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Administrador;

namespace SGCP.Application.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResult> GetAdmin();

        Task<ServiceResult> GetAdminById(int id);

        Task<ServiceResult> CreateAdmin(CreateAdminDTO createAdminDto);

        Task<ServiceResult> UpdateAdmin(UpdateAdminDTO updateAdminDto);

        Task<ServiceResult> RemoveAdmin(DeleteAdminDTO deleteAdminDto);
    }
}
