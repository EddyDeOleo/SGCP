using System.ComponentModel.DataAnnotations;

namespace SGCP.Web.Models.ModuloUsuarios.ClienteModels
{
    public class ClienteEditModel
    {
        // hacer que herede del base
        [Required(ErrorMessage = "El Id del admin es obligatorio.")]
        public int clienteId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(50)]
        public string apellido { get; set; }

        [Required(ErrorMessage = "El username es obligatorio.")]
        [MaxLength(50)]
        public string username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MaxLength(50)]
        public string password { get; set; }

    }

    public class Response_EC_Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}

