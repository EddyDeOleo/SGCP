
using SGCP.Application.Interfaces;

namespace SGCP.Application.Services
{
    public sealed class SessionService : ISessionService
    {
        public int? ClienteIdLogueado { get; private set; }
        public int? AdminIdLogueado { get; private set; }

        public void LoginCliente(int clienteId) => ClienteIdLogueado = clienteId;
        public void LogoutCliente() => ClienteIdLogueado = null;

        public void LoginAdmin(int adminId) => AdminIdLogueado = adminId;
        public void LogoutAdmin() => AdminIdLogueado = null;
    }
}
