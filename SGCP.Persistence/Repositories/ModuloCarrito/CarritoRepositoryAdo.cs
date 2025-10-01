using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Persistence.Base;
using SGCP.Persistence.Models.ModuloCarrito.Carrito;
using System.Data;
using System.Linq.Expressions;


namespace SGCP.Persistence.Repositories.ModuloCarrito
{
    public class CarritoRepositoryAdo : ICarrito
    {

        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<CarritoRepositoryAdo> _logger;

        public CarritoRepositoryAdo(IStoredProcedureExecutor spExecutor, ILogger<CarritoRepositoryAdo> logger)
        {
            _spExecutor = spExecutor;
            _logger = logger;
        }




        public async Task<bool> Exists(Expression<Func<Carrito, bool>> filter)
        {
            _logger.LogInformation("Verificando existencia de carritos con filtro {Filter}", filter);
            try
            {
                var result = await GetAll();
                if (!result.Success || result.Data == null)
                {
                    _logger.LogWarning("No se pudo obtener carritos para verificar existencia");
                    return false;
                }

                var lista = ((List<Carrito>)result.Data).AsQueryable();
                bool exists = lista.Any(filter.Compile());
                _logger.LogInformation("Existencia verificada: {Exists}", exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de carritos");
                throw;
            }
        }

        public async Task<OperationResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los carritos");
            try
            {
                var carritos = await _spExecutor.QueryAsync(
                    "sp_GetAllCarritos",
                    reader => new CarritoGetModel
                    {
                        IdCarrito = reader.GetInt32(reader.GetOrdinal("carrito_id")),
                        ClienteId = reader.GetInt32(reader.GetOrdinal("cliente_id"))
                    }
                );

                _logger.LogInformation("{Count} carritos obtenidos", carritos.Count);
                return OperationResult.SuccessResult("Carritos obtenidos correctamente", carritos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carritos");
                return OperationResult.FailureResult($"Error al obtener carritos: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetEntityBy(int id)
        {
            _logger.LogInformation("Obteniendo carrito con Id {Id}", id);
            try
            {
                var carritos = await _spExecutor.QueryAsync(
                    "sp_GetCarritoById",
                    reader => new CarritoGetModel
                    {
                        IdCarrito = reader.GetInt32(reader.GetOrdinal("carrito_id")),
                        ClienteId = reader.GetInt32(reader.GetOrdinal("cliente_id"))
                    },
                    new Dictionary<string, object> { { "@IdCarrito", id } }
                );

                if (!carritos.Any())
                {
                    _logger.LogWarning("Carrito con Id {Id} no encontrado", id);
                    return OperationResult.FailureResult("Carrito no encontrado");
                }

                _logger.LogInformation("Carrito con Id {Id} obtenido correctamente", id);
                return OperationResult.SuccessResult("Carrito obtenido correctamente", carritos.First());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carrito con Id {Id}", id);
                return OperationResult.FailureResult($"Error al obtener carrito: {ex.Message}");
            }
        }

        public async Task<OperationResult> Save(Carrito entity)
        {
            _logger.LogInformation("Creando carrito para ClienteId {ClienteId}", entity?.ClienteId);
            if (entity == null)
            {
                _logger.LogWarning("Carrito nulo no puede ser guardado");
                return OperationResult.FailureResult("El carrito no puede ser nulo.");
            }

            try
            {
                var parameters = new Dictionary<string, object> { { "@ClienteId", entity.ClienteId } };
                var outputParam = new SqlParameter("@IdCarrito", SqlDbType.Int) { Direction = ParameterDirection.Output };

                await _spExecutor.ExecuteAsync("sp_InsertCarrito", parameters, outputParam);
                entity.IdCarrito = (int)outputParam.Value;

                _logger.LogInformation("Carrito creado correctamente con Id {IdCarrito}", entity.IdCarrito);
                return OperationResult.SuccessResult("Carrito creado correctamente", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear carrito para ClienteId {ClienteId}", entity.ClienteId);
                return OperationResult.FailureResult($"Error al crear carrito: {ex.Message}");
            }
        }

        public async Task<OperationResult> Update(Carrito entity)
        {
            _logger.LogInformation("Actualizando carrito Id {IdCarrito}", entity?.IdCarrito);
            if (entity == null)
            {
                _logger.LogWarning("Carrito nulo no puede ser actualizado");
                return OperationResult.FailureResult("El carrito no puede ser nulo.");
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@IdCarrito", entity.IdCarrito },
                    { "@ClienteId", entity.ClienteId }
                };

                await _spExecutor.ExecuteAsync("sp_UpdateCarrito", parameters);

                _logger.LogInformation("Carrito actualizado correctamente Id {IdCarrito}", entity.IdCarrito);
                return OperationResult.SuccessResult("Carrito actualizado correctamente", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar carrito Id {IdCarrito}", entity.IdCarrito);
                return OperationResult.FailureResult($"Error al actualizar carrito: {ex.Message}");
            }
        }

        public async Task<OperationResult> Remove(Carrito entity)
        {
            _logger.LogInformation("Eliminando carrito Id {IdCarrito}", entity?.IdCarrito);
            if (entity == null)
            {
                _logger.LogWarning("Carrito nulo no puede ser eliminado");
                return OperationResult.FailureResult("El carrito no puede ser nulo.");
            }

            try
            {
                var parameters = new Dictionary<string, object> { { "@IdCarrito", entity.IdCarrito } };
                await _spExecutor.ExecuteAsync("sp_DeleteCarrito", parameters);

                _logger.LogInformation("Carrito eliminado correctamente Id {IdCarrito}", entity.IdCarrito);
                return OperationResult.SuccessResult("Carrito eliminado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar carrito Id {IdCarrito}", entity.IdCarrito);
                return OperationResult.FailureResult($"Error al eliminar carrito: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetAll(Expression<Func<Carrito, bool>> filter)
        {
            _logger.LogInformation("Obteniendo carritos filtrados");
            try
            {
                var result = await _spExecutor.QueryAsync(
                    "sp_GetAllCarritos",
                    reader => new CarritoGetModel
                    {
                        IdCarrito = reader.GetInt32(reader.GetOrdinal("carrito_id")),
                        ClienteId = reader.GetInt32(reader.GetOrdinal("cliente_id"))
                    }
                );

                if (!result.Any())
                {
                    _logger.LogWarning("No se pudieron obtener carritos");
                    return OperationResult.FailureResult("No se pudieron obtener carritos");
                }

                var carritos = result.Select(model => new Carrito
                {
                    IdCarrito = model.IdCarrito,
                    ClienteId = model.ClienteId,
                    Productos = new List<Producto>(),
                    Cantidades = new Dictionary<Producto, int>()
                }).AsQueryable();

                var listaFiltrada = carritos.Where(filter.Compile()).ToList();

                _logger.LogInformation("{Count} carritos filtrados obtenidos", listaFiltrada.Count);
                return OperationResult.SuccessResult("Carritos filtrados correctamente", listaFiltrada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carritos filtrados");
                return OperationResult.FailureResult($"Error al obtener carritos filtrados: {ex.Message}");
            }
        }

        public async Task<Carrito> GetByClienteId(int clienteId)
        {
            _logger.LogInformation("Obteniendo carrito del cliente Id {ClienteId}", clienteId);
            try
            {
                var result = await _spExecutor.QueryAsync(
                    "sp_GetCarritoByClienteId",
                    reader => new CarritoGetModel
                    {
                        IdCarrito = reader.GetInt32(reader.GetOrdinal("carrito_id")),
                        ClienteId = reader.GetInt32(reader.GetOrdinal("cliente_id"))
                    },
                    new Dictionary<string, object> { { "@ClienteId", clienteId } }
                );

                var model = result.FirstOrDefault();
                if (model == null)
                {
                    _logger.LogWarning("No se encontró carrito para cliente Id {ClienteId}", clienteId);
                    return null;
                }

                var carrito = new Carrito
                {
                    IdCarrito = model.IdCarrito,
                    ClienteId = model.ClienteId,
                    Productos = new List<Producto>(),
                    Cantidades = new Dictionary<Producto, int>()
                };

                _logger.LogInformation("Carrito obtenido para cliente Id {ClienteId}", clienteId);
                return carrito;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carrito para cliente Id {ClienteId}", clienteId);
                throw;
            }
        }

    }
}
