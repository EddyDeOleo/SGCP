namespace SGCP.Web.Models.ModuloUsuarios.AdministradorModels
{
    public class AdminGetModel
    {

        public int adminId { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string username { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime? fechaModificacion { get; set; }
        public int? usuarioModificacion { get; set; }
        public bool estatus { get; set; }
        public string password { get; set; }

    }

    

    public class Response_GAA_Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<AdminGetModel> data { get; set; }
    }

    public class Response_GA_Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public AdminGetModel data { get; set; }
    }
}
