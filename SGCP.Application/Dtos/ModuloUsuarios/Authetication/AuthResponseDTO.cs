

namespace SGCP.Application.Dtos.ModuloUsuarios.Authetication
{
    public record AuthResponseDTO : AuthBaseDTO
    {
        public int UserId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
