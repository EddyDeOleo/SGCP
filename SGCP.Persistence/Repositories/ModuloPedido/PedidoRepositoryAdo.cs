using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloPedido;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Persistence.Base;
using SGCP.Persistence.Models.ModuloPedido.Pedido;
using System.Data;
using System.Linq.Expressions;

namespace SGCP.Persistence.Repositories.ModuloPedido
{
    public class PedidoRepositoryAdo : IPedido
    {

        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<PedidoRepositoryAdo> _logger;

        public PedidoRepositoryAdo(IStoredProcedureExecutor spExecutor, ILogger<PedidoRepositoryAdo> logger)
        {
            _spExecutor = spExecutor;
            _logger = logger;
        }

        public async Task<bool> Exists(Expression<Func<Pedido, bool>> filter)
        {
            _logger.LogInformation("Verificando existencia de pedidos con filtro {Filter}", filter);
            try
            {
                var result = await GetAll();
                if (!result.Success || result.Data == null)
                {
                    _logger.LogWarning("No se pudo obtener pedidos para verificar existencia");
                    return false;
                }

                var lista = ((List<PedidoGetModel>)result.Data)
                    .Select(pgm => new Pedido(pgm.Carrito, pgm.Cliente)
                    {
                        IdPedido = pgm.IdPedido,
                        Total = pgm.Total,
                        Estado = pgm.Estado
                    })
                    .AsQueryable();

                bool exists = lista.Any(filter.Compile());
                _logger.LogInformation("Existencia verificada: {Exists}", exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de pedidos");
                throw;
            }
        }

        public async Task<OperationResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los pedidos");
            try
            {
                var pedidos = await _spExecutor.QueryAsync(
                    "sp_GetAllPedidos",
                    reader => new PedidoGetModel
                    {
                        IdPedido = reader.GetInt32(reader.GetOrdinal("pedido_id")),
                        Cliente = new Cliente(reader.GetInt32(reader.GetOrdinal("cliente_id")), "", "", "", ""),
                        Carrito = new Carrito { IdCarrito = reader.GetInt32(reader.GetOrdinal("carrito_id")) },
                        Total = reader.GetDecimal(reader.GetOrdinal("total")),
                        Estado = reader.GetString(reader.GetOrdinal("estado"))
                    }
                );

                _logger.LogInformation("{Count} pedidos obtenidos", pedidos.Count);
                return OperationResult.SuccessResult("Pedidos obtenidos correctamente", pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos");
                return OperationResult.FailureResult($"Error al obtener pedidos: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetEntityBy(int id)
        {
            _logger.LogInformation("Obteniendo pedido con Id {Id}", id);
            try
            {
                var pedidos = await _spExecutor.QueryAsync(
                    "sp_GetPedidoById",
                    reader => new PedidoGetModel
                    {
                        IdPedido = reader.GetInt32(reader.GetOrdinal("pedido_id")),
                        Cliente = new Cliente(reader.GetInt32(reader.GetOrdinal("cliente_id")), "", "", "", ""),
                        Carrito = new Carrito { IdCarrito = reader.GetInt32(reader.GetOrdinal("carrito_id")) },
                        Total = reader.GetDecimal(reader.GetOrdinal("total")),
                        Estado = reader.GetString(reader.GetOrdinal("estado"))
                    },
                    new Dictionary<string, object> { { "@IdPedido", id } }
                );

                if (!pedidos.Any())
                {
                    _logger.LogWarning("Pedido con Id {Id} no encontrado", id);
                    return OperationResult.FailureResult("Pedido no encontrado");
                }

                _logger.LogInformation("Pedido con Id {Id} obtenido correctamente", id);
                return OperationResult.SuccessResult("Pedido obtenido correctamente", pedidos.First());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedido con Id {Id}", id);
                return OperationResult.FailureResult($"Error al obtener pedido: {ex.Message}");
            }
        }

        public async Task<OperationResult> Save(Pedido entity)
        {
            _logger.LogInformation("Creando pedido para ClienteId {ClienteId}", entity?.Cliente?.IdUsuario);

            if (entity == null)
            {
                _logger.LogWarning("Pedido nulo no puede ser guardado");
                return OperationResult.FailureResult("El pedido no puede ser nulo.");
            }
            if (entity.Cliente == null)
            {
                _logger.LogWarning("Cliente nulo no puede ser guardado en pedido");
                return OperationResult.FailureResult("El cliente es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(entity.Estado) || entity.Estado.Length > 30)
            {
                _logger.LogWarning("Estado inválido para pedido");
                return OperationResult.FailureResult("El estado del pedido es obligatorio y no puede exceder 30 caracteres.");
            }
            if (entity.Total < 0)
            {
                _logger.LogWarning("Total negativo no permitido en pedido");
                return OperationResult.FailureResult("El total del pedido no puede ser negativo.");
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ClienteId", entity.Cliente.IdUsuario },
                    { "@CarritoId", entity.Carrito.IdCarrito },
                    { "@Total", entity.Total },
                    { "@Estado", entity.Estado }
                };

                var outputParam = new SqlParameter("@IdPedido", SqlDbType.Int) { Direction = ParameterDirection.Output };
                await _spExecutor.ExecuteAsync("sp_InsertPedido", parameters, outputParam);
                entity.IdPedido = (int)outputParam.Value;

                _logger.LogInformation("Pedido creado correctamente con Id {IdPedido}", entity.IdPedido);
                return OperationResult.SuccessResult("Pedido creado correctamente", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pedido para ClienteId {ClienteId}", entity.Cliente.IdUsuario);
                return OperationResult.FailureResult($"Error al crear pedido: {ex.Message}");
            }
        }

        public async Task<OperationResult> Update(Pedido entity)
        {
            _logger.LogInformation("Actualizando pedido Id {IdPedido}", entity?.IdPedido);

            if (entity == null || entity.Cliente == null || string.IsNullOrWhiteSpace(entity.Estado) || entity.Estado.Length > 30 || entity.Total < 0)
            {
                _logger.LogWarning("Datos inválidos para actualizar pedido Id {IdPedido}", entity?.IdPedido);
                return OperationResult.FailureResult("Datos inválidos para actualizar el pedido");
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@IdPedido", entity.IdPedido },
                    { "@ClienteId", entity.Cliente.IdUsuario },
                    { "@CarritoId", entity.Carrito.IdCarrito },
                    { "@Total", entity.Total },
                    { "@Estado", entity.Estado }
                };

                await _spExecutor.ExecuteAsync("sp_UpdatePedido", parameters);
                _logger.LogInformation("Pedido actualizado correctamente Id {IdPedido}", entity.IdPedido);
                return OperationResult.SuccessResult("Pedido actualizado correctamente", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar pedido Id {IdPedido}", entity.IdPedido);
                return OperationResult.FailureResult($"Error al actualizar pedido: {ex.Message}");
            }
        }

        public async Task<OperationResult> Remove(Pedido entity)
        {
            _logger.LogInformation("Eliminando pedido Id {IdPedido}", entity?.IdPedido);
            if (entity == null)
            {
                _logger.LogWarning("Pedido nulo no puede ser eliminado");
                return OperationResult.FailureResult("El pedido no puede ser nulo.");
            }

            try
            {
                var parameters = new Dictionary<string, object> { { "@IdPedido", entity.IdPedido } };
                await _spExecutor.ExecuteAsync("sp_DeletePedido", parameters);

                _logger.LogInformation("Pedido eliminado correctamente Id {IdPedido}", entity.IdPedido);
                return OperationResult.SuccessResult("Pedido eliminado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar pedido Id {IdPedido}", entity.IdPedido);
                return OperationResult.FailureResult($"Error al eliminar pedido: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetAll(Expression<Func<Pedido, bool>> filter)
        {
            _logger.LogInformation("Obteniendo pedidos filtrados");
            try
            {
                var result = await GetAll();
                if (!result.Success || result.Data == null)
                {
                    _logger.LogWarning("No se pudieron obtener pedidos");
                    return OperationResult.FailureResult("No se pudieron obtener pedidos");
                }

                var pedidos = ((List<PedidoGetModel>)result.Data)
                    .Select(pgm => new Pedido(pgm.Carrito, pgm.Cliente)
                    {
                        IdPedido = pgm.IdPedido,
                        Total = pgm.Total,
                        Estado = pgm.Estado
                    })
                    .AsQueryable()
                    .Where(filter.Compile())
                    .ToList();

                _logger.LogInformation("{Count} pedidos filtrados obtenidos", pedidos.Count);
                return OperationResult.SuccessResult("Pedidos filtrados correctamente", pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos filtrados");
                return OperationResult.FailureResult($"Error al obtener pedidos filtrados: {ex.Message}");
            }
        }

        public async Task<List<Pedido>> GetPedidosByClienteId(int clienteId)
        {
            _logger.LogInformation("Obteniendo pedidos para ClienteId {ClienteId}", clienteId);
            try
            {
                var parameters = new Dictionary<string, object> { { "@ClienteId", clienteId } };

                var pedidosGet = await _spExecutor.QueryAsync(
                    "sp_GetPedidosByClienteId",
                    reader => new PedidoGetModel
                    {
                        IdPedido = reader.GetInt32(reader.GetOrdinal("pedido_id")),
                        Cliente = new Cliente(reader.GetInt32(reader.GetOrdinal("cliente_id")), "", "", "", ""),
                        Carrito = new Carrito { IdCarrito = reader.GetInt32(reader.GetOrdinal("carrito_id")) },
                        Total = reader.GetDecimal(reader.GetOrdinal("total")),
                        Estado = reader.GetString(reader.GetOrdinal("estado"))
                    },
                    parameters
                );

                var pedidos = pedidosGet.Select(pgm => new Pedido(pgm.Carrito, pgm.Cliente)
                {
                    IdPedido = pgm.IdPedido,
                    Total = pgm.Total,
                    Estado = pgm.Estado
                }).ToList();

                _logger.LogInformation("{Count} pedidos obtenidos para ClienteId {ClienteId}", pedidos.Count, clienteId);
                return pedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos para ClienteId {ClienteId}", clienteId);
                throw;
            }
            }
        }
}
