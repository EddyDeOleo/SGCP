

using Microsoft.Extensions.Logging;
using SGCP.Application.Dtos.ModuloReporte.Reporte;
using SGCP.Application.Repositories.ModuloReporte;
using SGCP.Application.Repositories.ModuloUsuarios;

namespace SGCP.Application.Base.ServiceValidator.ModuloReporte
{
    public class ReporteServiceValidator : ServiceValidator<ReporteServiceValidator>
    {
        private readonly IReporte _reporteRepository;
        private readonly IAdministrador _adminRepository;

        public ReporteServiceValidator(
            ILogger<ReporteServiceValidator> logger,
            IReporte reporteRepository,
            IAdministrador adminRepository
        ) : base(logger)
        {
            _reporteRepository = reporteRepository;
            _adminRepository = adminRepository;
        }

       
        public ServiceResult ValidateForCreate(CreateReporteDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de creación de reporte");
            if (!dtoVal.Success) return dtoVal;

            var adminIdVal = ValidateId(dto.AdminId, "AdminId");
            if (!adminIdVal.Success) return adminIdVal;

            return Success("DTO válido para crear reporte");
        }

        public ServiceResult ValidateForUpdate(UpdateReporteDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de actualización de reporte");
            if (!dtoVal.Success) return dtoVal;

            var idVal = ValidateId(dto.IdReporte, "IdReporte");
            if (!idVal.Success) return idVal;

            var adminIdVal = ValidateId(dto.AdminId, "AdminId");
            if (!adminIdVal.Success) return adminIdVal;

            return Success("DTO válido para actualizar reporte");
        }

        public ServiceResult ValidateForDelete(DeleteReporteDTO dto)
        {
            var dtoVal = ValidateNotNull(dto, "DTO de eliminación de reporte");
            if (!dtoVal.Success) return dtoVal;

            var idVal = ValidateId(dto.IdReporte, "IdReporte");
            if (!idVal.Success) return idVal;

            return Success("DTO válido para eliminar reporte");
        }

       
        public async Task<ServiceResult> ValidateReporteExistente(int idReporte)
        {
            var idVal = ValidateId(idReporte, "IdReporte");
            if (!idVal.Success) return idVal;

            var result = await _reporteRepository.GetEntityBy(idReporte);
            if (!result.Success || result.Data == null)
                return Failure("Reporte no existe");

            return Success("Reporte existente", result.Data);
        }

        public async Task<ServiceResult> ValidateAdministradorExistente(int adminId)
        {
            var idVal = ValidateId(adminId, "AdminId");
            if (!idVal.Success) return idVal;

            var result = await _adminRepository.GetEntityBy(adminId);
            if (!result.Success || result.Data == null)
                return Failure("Administrador no existe");

            return Success("Administrador válido", result.Data);
        }
    }
}
