using SGCP.Web.Models.ModuloUsuarios.AdministradorModels;

namespace SGCP.Web.Models.ModuloUsuarios.ClienteModels
{
    public class ClienteGetModel
    {

        public int clienteId { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string username { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime? fechaModificacion { get; set; }
        public int? usuarioModificacion { get; set; }
        public bool estatus { get; set; }
        public string password { get; set; }

    }



    public class Response_GAC_Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<ClienteGetModel> data { get; set; }
    }

    public class Response_GC_Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public ClienteGetModel data { get; set; }
    }
}
