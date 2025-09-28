using SGCP.Domain.Repository;  
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Repositories
{
    internal interface IUsuario : IBaseRepository<Usuario>
    {
        Task<Usuario> GetByUsername(string username);

    }
}
