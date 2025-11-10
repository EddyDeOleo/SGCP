
using Microsoft.Extensions.Logging;
using SGCP.Application.Dtos.ModuloCarrito.CarritoProducto;
using SGCP.Application.Dtos.ModuloPedido.Pedido;
using SGCP.Application.Interfaces.IServiceValidator.ModuloPedido;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloPedido;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDePedido;

namespace SGCP.Application.Base.ServiceValidator.ModuloPedido
{
    public class PedidoServiceValidator : ServiceValidator<PedidoServiceValidator>, IPedidoServiceValidator
    {
        private readonly IPedido _pedidoRepository;
        private readonly ICarrito _carritoRepository;
        private readonly ICliente _clienteRepository;
        private readonly ICarritoProducto _carritoProductoRepo;

        public PedidoServiceValidator(
            ILogger<PedidoServiceValidator> logger,
            IPedido pedidoRepository,
            ICarrito carritoRepository,
            ICliente clienteRepository,
            ICarritoProducto carritoProductoRepo
        ) : base(logger)
        {
            _pedidoRepository = pedidoRepository;
            _carritoRepository = carritoRepository;
            _clienteRepository = clienteRepository;
            _carritoProductoRepo = carritoProductoRepo;
        }

        // -------------------------
        // Validaciones de DTO
        // -------------------------
        public ServiceResult ValidateForCreate(CreatePedidoDTO dto)
        {
            var dtoResult = ValidateNotNull(dto, "DTO de creación de pedido");
            if (!dtoResult.Success) return dtoResult;

            var clienteVal = ValidateId(dto.ClienteId, "ClienteId");
            if (!clienteVal.Success) return clienteVal;

            var carritoVal = ValidateId(dto.CarritoId, "CarritoId");
            if (!carritoVal.Success) return carritoVal;

            return Success("DTO válido para crear pedido");
        }

        public ServiceResult ValidateForUpdate(UpdatePedidoDTO dto)
            => ValidateForUpdate(dto);

        public ServiceResult ValidateForDelete(DeletePedidoDTO dto)
            => ValidateForDelete(dto);

        // -------------------------
        // Validaciones repositorio
        // -------------------------
        public async Task<ServiceResult> ValidateCliente(int id)
        {
            var idVal = ValidateId(id, "ClienteId");
            if (!idVal.Success) return idVal;

            var result = await _clienteRepository.GetEntityBy(id);
            return result.Success && result.Data != null
                ? Success("Cliente válido")
                : Failure("El cliente no existe.");
        }

        public async Task<ServiceResult> ValidateCarrito(int id)
        {
            var idVal = ValidateId(id, "CarritoId");
            if (!idVal.Success) return idVal;

            var result = await _carritoRepository.GetEntityBy(id);
            return result.Success && result.Data != null
                ? Success("Carrito válido")
                : Failure("El carrito no existe.");
        }

        public async Task<ServiceResult> ValidateProductosCarrito(int idCarrito)
        {
            var idVal = ValidateId(idCarrito, "CarritoId");
            if (!idVal.Success) return idVal;

            var result = await _carritoProductoRepo.GetProductosByCarritoId(idCarrito);
            var productos = result.Data as List<CarritoProductoGetDTO>;

            if (productos == null || !productos.Any())
                return Failure("El carrito no tiene productos.");

            return Success("Productos del carrito válidos.", productos);
        }

        public async Task<ServiceResult> ValidatePedidoExistente(int idPedido)
        {
            var idVal = ValidateId(idPedido, "IdPedido");
            if (!idVal.Success) return idVal;

            var result = await _pedidoRepository.GetEntityBy(idPedido);
            if (!result.Success || result.Data == null)
                return Failure("El pedido no existe.");

            return Success("Pedido existente.", result.Data);
        }

        // -------------------------
        // Validaciones de estado
        // -------------------------
        public ServiceResult ValidateEstadoParaFinalizar(Pedido pedido)
        {
            if (pedido.Estado == "Finalizado")
                return Failure("El pedido ya está finalizado.");
            if (pedido.Estado == "Cancelado")
                return Failure("No se puede finalizar un pedido cancelado.");
            return Success("Estado válido para finalizar.");
        }

        public ServiceResult ValidateEstadoParaCancelar(Pedido pedido)
        {
            if (pedido.Estado == "Cancelado")
                return Failure("El pedido ya está cancelado.");
            if (pedido.Estado == "Finalizado")
                return Failure("No se puede cancelar un pedido finalizado.");
            return Success("Estado válido para cancelar.");
        }
    }

}
