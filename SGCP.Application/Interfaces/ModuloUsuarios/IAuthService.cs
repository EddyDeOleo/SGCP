using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Authetication;

namespace SGCP.Application.Interfaces.ModuloUsuarios
{
    public interface IAuthService
    {
        Task<ServiceResult> Login(LoginDTO loginDto);
        Task<ServiceResult> Logout(int userId);
    }
}
