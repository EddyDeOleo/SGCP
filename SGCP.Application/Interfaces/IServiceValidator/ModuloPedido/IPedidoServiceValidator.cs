using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloCarrito.Carrito;
using SGCP.Application.Dtos.ModuloPedido.Pedido;
using SGCP.Domain.Entities.ModuloDePedido;


namespace SGCP.Application.Interfaces.IServiceValidator.ModuloPedido
{
    public interface IPedidoServiceValidator : IServiceValidatorBase<CreatePedidoDTO, UpdatePedidoDTO, DeletePedidoDTO>
    {
    
   
        Task<ServiceResult> ValidateCliente(int id);
        Task<ServiceResult> ValidateCarrito(int id);
        Task<ServiceResult> ValidateProductosCarrito(int idCarrito);
        Task<ServiceResult> ValidatePedidoExistente(int idPedido);

        ServiceResult ValidateEstadoParaFinalizar(Pedido pedido);
        ServiceResult ValidateEstadoParaCancelar(Pedido pedido);
    }
}
