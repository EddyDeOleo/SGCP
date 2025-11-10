using SGCP.Application.Base;


namespace SGCP.Application.Interfaces.IServiceValidator
{
    public interface IServiceValidatorBase<TCreateDTO, TUpdateDTO, TDeleteDTO>
    {
        ServiceResult ValidateForCreate(TCreateDTO dto);
        ServiceResult ValidateForUpdate(TUpdateDTO dto);
        ServiceResult ValidateForDelete(TDeleteDTO dto);
    }
}
