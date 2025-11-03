

namespace SGCP.Domain.Entities.ModuloDeReporte
{
    public sealed class Reporte : Base.BaseEntity
    {
        public int IdReporte { get; set; }
        public int AdminId { get; set; }
        public decimal TotalVentas { get; set; }
        public int TotalPedidos { get; set; }

 



    }
}
