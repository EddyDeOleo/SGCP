using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SGCP.Web.Models.ModuloUsuarios.AuthModels
{
    public class AuthLoginModel
    {

        [Required(ErrorMessage = "El username es obligatorio.")]
        [MaxLength(50)]
        public string username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MaxLength(50)]
        public string password { get; set; }

    }

    public class Response_L_Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public JsonElement data { get; set; }
    }
}
