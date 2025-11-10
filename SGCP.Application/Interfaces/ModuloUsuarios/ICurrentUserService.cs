namespace SGCP.Application.Interfaces.ModuloUsuarios
{
    public interface ICurrentUserService
    {
        int? GetUserId();
        string GetUserName();
    }
}
