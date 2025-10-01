
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Domain.Repository;
using System;
using System.Linq.Expressions;

namespace SGCP.Persistence.Repositories.Base
{
    public abstract class BaseRepositoryEF<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly SGCPDbContext _context;
        protected readonly ILogger _logger;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepositoryEF(SGCPDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<OperationResult> Save(TEntity entity)
        {
            _logger.LogInformation("Guardando {EntityType}", typeof(TEntity).Name);
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("{EntityType} guardado correctamente", typeof(TEntity).Name);
                return OperationResult.SuccessResult($"{typeof(TEntity).Name} guardado correctamente", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar {EntityType}", typeof(TEntity).Name);
                return OperationResult.FailureResult($"Error al guardar {typeof(TEntity).Name}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult> Update(TEntity entity)
        {
            _logger.LogInformation("Actualizando {EntityType}", typeof(TEntity).Name);
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("{EntityType} actualizado correctamente", typeof(TEntity).Name);
                return OperationResult.SuccessResult($"{typeof(TEntity).Name} actualizado correctamente", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar {EntityType}", typeof(TEntity).Name);
                return OperationResult.FailureResult($"Error al actualizar {typeof(TEntity).Name}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult> Remove(TEntity entity)
        {
            _logger.LogInformation("Eliminando {EntityType}", typeof(TEntity).Name);
            try
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("{EntityType} eliminado correctamente", typeof(TEntity).Name);
                return OperationResult.SuccessResult($"{typeof(TEntity).Name} eliminado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar {EntityType}", typeof(TEntity).Name);
                return OperationResult.FailureResult($"Error al eliminar {typeof(TEntity).Name}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult> GetAll()
        {
            try
            {
                var list = await _dbSet.ToListAsync();
                return OperationResult.SuccessResult($"{typeof(TEntity).Name}s obtenidos correctamente", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener {EntityType}s", typeof(TEntity).Name);
                return OperationResult.FailureResult($"Error al obtener {typeof(TEntity).Name}s: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult> GetAll(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                var list = await _dbSet.Where(filter).ToListAsync();
                return OperationResult.SuccessResult($"{typeof(TEntity).Name}s filtrados correctamente", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar {EntityType}s", typeof(TEntity).Name);
                return OperationResult.FailureResult($"Error al filtrar {typeof(TEntity).Name}s: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult> GetEntityBy(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    return OperationResult.FailureResult($"{typeof(TEntity).Name} no encontrado");

                return OperationResult.SuccessResult($"{typeof(TEntity).Name} obtenido correctamente", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener {EntityType}", typeof(TEntity).Name);
                return OperationResult.FailureResult($"Error al obtener {typeof(TEntity).Name}: {ex.Message}");
            }
        }

        public virtual async Task<bool> Exists(Expression<Func<TEntity, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }
    }
}
