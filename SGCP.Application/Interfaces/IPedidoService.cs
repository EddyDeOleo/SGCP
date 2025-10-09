
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloPedido.Pedido;  

namespace SGCP.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<ServiceResult> GetPedido();

        Task<ServiceResult> GetPedidoById(int id);

        Task<ServiceResult> CreatePedido(CreatePedidoDTO createPedidoDto);

        Task<ServiceResult> UpdatePedido(UpdatePedidoDTO updatePedidoDto);

        Task<ServiceResult> RemovePedido(DeletePedidoDTO deletePedidoDto);
    }
}