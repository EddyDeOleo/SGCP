

namespace SGCP.Application.Dtos.ModuloCarrito.Carrito
{
    public record CarritoGetDTO : CarritoBaseDTO
    {
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public bool Estatus { get; set; }
    }
}
