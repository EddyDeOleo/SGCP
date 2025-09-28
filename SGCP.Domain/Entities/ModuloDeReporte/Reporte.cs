
using SGCP.Domain.Entities.ModuloDePedido;

namespace SGCP.Domain.Entities.ModuloDeReporte
{
    public sealed class Reporte : Base.BaseEntity
    {
        public int IdReporte { get; private set; }
        public decimal TotalVentas { get; private set; }
        public int TotalPedidos { get; private set; }

        public void GenerarReporte(List<Pedido> pedidos)
        {
            TotalVentas = pedidos.Sum(p => p.Total);
            TotalPedidos = pedidos.Count;
        }



    }
}
