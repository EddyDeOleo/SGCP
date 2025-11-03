
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGCP.Domain.Entities.ModuloDeUsuarios
{
    public abstract class Usuario : Base.BaseEntity
    {
        [Key]
        [Column("usuario_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get;  set; }
        public string Username { get; set; }
        public string Password { get; set; }


    


        protected Usuario() { }

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

  
    }
}
