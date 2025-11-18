using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Domain.Repository;
using SGCP.Persistence.Base.IEntityValidator;
using SGCP.Persistence.Repositories.ModuloUsuarios;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace SGCP.Persistence.Repositories.Base
{
    public abstract class BaseRepositoryEF<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly SGCPDbContext _context;
        protected readonly ILogger _logger;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly IEntityValidator<TEntity> _validator;

        public BaseRepositoryEF(SGCPDbContext context, ILogger logger, IEntityValidator<TEntity> validator)
        {
            _context = context;
            _logger = logger;
            _dbSet = _context.Set<TEntity>();
            _validator = validator;
        }

        protected BaseRepositoryEF(SGCPDbContext context, ILogger<UsuarioRepositoryEF> logger)
        {
            _context = context;
            _logger = logger;
        }

        private OperationResult ValidateAnnotations(TEntity entity)
        {
            var ctx = new ValidationContext(entity);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, ctx, results, true);

            if (!isValid)
            {
                var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
                return OperationResult.FailureResult($"Error de validación: {errors}");
            }

            return OperationResult.SuccessResult("OK");
        }

        public virtual async Task<OperationResult> Save(TEntity entity)
        {
            _logger.LogInformation("Guardando {Entity}", typeof(TEntity).Name);

            var validation = _validator.ValidateForSave(entity);
            if (!validation.Success)
                return validation;

            var annotations = ValidateAnnotations(entity);
            if (!annotations.Success)
                return annotations;

            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

                return OperationResult.SuccessResult(
                    $"{typeof(TEntity).Name} guardado correctamente",
                    entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar {Entity}", typeof(TEntity).Name);
                return OperationResult.FailureResult(ex.Message);
            }
        }

        public virtual async Task<OperationResult> Update(TEntity entity)
        {
            _logger.LogInformation("Actualizando {Entity}", typeof(TEntity).Name);

            var validation = _validator.ValidateForUpdate(entity);
            if (!validation.Success)
                return validation;

            var annotations = ValidateAnnotations(entity);
            if (!annotations.Success)
                return annotations;

            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();

                return OperationResult.SuccessResult(
                    $"{typeof(TEntity).Name} actualizado correctamente",
                    entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar {Entity}", typeof(TEntity).Name);
                return OperationResult.FailureResult(ex.Message);
            }
        }

        public virtual async Task<OperationResult> Remove(TEntity entity)
        {
            _logger.LogInformation("Eliminando {Entity}", typeof(TEntity).Name);

            var validation = _validator.ValidateForRemove(entity);
            if (!validation.Success)
                return validation;

            try
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();

                return OperationResult.SuccessResult(
                    $"{typeof(TEntity).Name} eliminado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar {Entity}", typeof(TEntity).Name);
                return OperationResult.FailureResult(ex.Message);
            }
        }

        public virtual async Task<OperationResult> GetAll()
        {
            try
            {
                var list = await _dbSet.ToListAsync();
                return OperationResult.SuccessResult("OK", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAll");
                return OperationResult.FailureResult(ex.Message);
            }
        }

        public virtual async Task<OperationResult> GetAll(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                var list = await _dbSet.Where(filter).ToListAsync();
                return OperationResult.SuccessResult("OK", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAll filtrado");
                return OperationResult.FailureResult(ex.Message);
            }
        }

        public virtual async Task<OperationResult> GetEntityBy(int id)
        {
            _logger.LogInformation("Obteniendo {Entity} por Id {Id}", typeof(TEntity).Name, id);


            try
            {
                var entity = await _dbSet.FindAsync(id);

                if (entity == null)
                    return OperationResult.FailureResult($"{typeof(TEntity).Name} no encontrado");

                return OperationResult.SuccessResult("OK", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetEntityBy");
                return OperationResult.FailureResult(ex.Message);
            }
        }

        public virtual async Task<bool> Exists(Expression<Func<TEntity, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }
    }
}

