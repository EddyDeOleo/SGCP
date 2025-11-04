
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
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(50)]
        public string Apellido { get;  set; }

        [Required(ErrorMessage = "El username es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El username no puede exceder los 50 caracteres.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MaxLength(255, ErrorMessage = "La contraseña no puede exceder los 255 caracteres.")]
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
