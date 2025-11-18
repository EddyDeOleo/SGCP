

using SGCP.Domain.Base;

namespace SGCP.Persistence.Base.IEntityValidator
{
    public interface IEntityValidator<TEntity> where TEntity : class
    {
        OperationResult ValidateForSave(TEntity entity);
        OperationResult ValidateForUpdate(TEntity entity);
        OperationResult ValidateForRemove(TEntity entity);
    }
}
