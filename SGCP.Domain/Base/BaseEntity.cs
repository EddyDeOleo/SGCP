
using System.ComponentModel.DataAnnotations.Schema;

namespace SGCP.Domain.Base
{
    public abstract class BaseEntity
    {
        [NotMapped]
        public DateTime FechaCreacion { get; set; }
        [NotMapped]

        public DateTime? FechaModificacion { get; set; }
        [NotMapped]

        public int? UsuarioModificacion { get; set; }
        [NotMapped]

        public bool Estatus { get; set; }
    }
}
