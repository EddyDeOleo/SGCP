
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGCP.Domain.Entities.ModuloDeUsuarios
{
    public abstract class Usuario 
    {
        [Key]
        [Column("usuario_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get;  set; }
        public string Username { get; set; }
        public string Password { get; set; }


        protected Usuario(int idUsuario, string nombre, string apellido, string username, string password)
        {
            IdUsuario = idUsuario;
            Nombre = nombre;
            Apellido = apellido;
            Username = username;
            Password = password;
        }

        protected Usuario(string nombre, string apellido, string username, string password)
        {
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
