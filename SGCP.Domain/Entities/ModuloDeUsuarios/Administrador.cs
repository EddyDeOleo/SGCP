using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Domain.Entities.ModuloDeReporte;

namespace SGCP.Domain.Entities.ModuloDeUsuarios
{
    public sealed class Administrador : Usuario
    {


        public Administrador(int idUsuario, string nombre, string apellido, string username, string password)
           : base(idUsuario, nombre, apellido, username, password) { }

        public Administrador(string nombre, string apellido, string username, string password) : base(nombre, apellido, username, password)
        {
        }

    
    }
}
