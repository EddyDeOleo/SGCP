
namespace SGCP.Domain.Entities.ModuloDeUsuarios
{
    public abstract class Usuario : Base.BaseEntity
    {
        public int IdUsuario { get; private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        protected Usuario(int idUsuario, string nombre, string apellido, string username, string password)
        {
            IdUsuario = idUsuario;
            Nombre = nombre;
            Apellido = apellido;
            Username = username;
            Password = password;
        }

        public virtual bool IniciarSesion(string username, string password)
        {
            if (Username == username && Password == password)
            {
                Console.WriteLine($"Usuario {username} ha iniciado sesión.");
                return true;
            }
            Console.WriteLine("Credenciales incorrectas.");
            return false;
        }

        public virtual void CerrarSesion()
        {
            Console.WriteLine($"Usuario {Username} ha cerrado sesión.");
        }
    }
}
