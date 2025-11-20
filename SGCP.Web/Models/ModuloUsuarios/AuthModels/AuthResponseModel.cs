
namespace SGCP.Web.Models.ModuloUsuarios.AuthModels
{
    public class AuthResponseModel
    {
            public int UserId { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
        
        public string Username { get; set; }
    }
}
