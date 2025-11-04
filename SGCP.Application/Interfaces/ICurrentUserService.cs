
namespace SGCP.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int? GetUserId();
        string GetUserName();
    }
}
