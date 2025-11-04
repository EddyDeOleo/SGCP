

namespace SGCP.Infraestructure.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(int userId, string username, string nombre, string apellido);
    }
}
