using SGCP.Domain.Base;

namespace SGCP.Persistence.Base.EntityValidator
{
    public interface IEntityValidator<T>
    {
        OperationResult ValidateForSave(T entity);
        OperationResult ValidateForUpdate(T entity);
        OperationResult ValidateForRemove(T entity);
    }
}
