using SGCP.Domain.Repository;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Repositories.ModuloUsuarios
{
    public interface IUsuario : IBaseRepository<Usuario>
    {
        Task<Usuario> GetByUsername(string username);

    }
}
