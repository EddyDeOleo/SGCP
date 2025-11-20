
using System.ComponentModel.DataAnnotations;

namespace SGCP.Domain.Base
{
    public abstract class BaseEntity
    {
        [Required]
        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int? UsuarioModificacion { get; set; }

        public bool Estatus { get; set; }
    }
}
