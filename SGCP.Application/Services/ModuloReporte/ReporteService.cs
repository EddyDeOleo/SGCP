
using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Base.ServiceValidator.ModuloReporte;
using SGCP.Application.Dtos.ModuloReporte.Reporte;
using SGCP.Application.Interfaces;
using SGCP.Application.Mappers;
using SGCP.Application.Repositories.ModuloReporte;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeReporte;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Services.ModuloReporte
    {
    public sealed class ReporteService : BaseService<ReporteService>, IReporteService
    {
        private readonly IReporte _reporteRepository;
        private readonly IAdministrador _adminRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ReporteServiceValidator _reporteServiceValidator;

        public ReporteService(
            IReporte reporteRepository,
            ILogger<ReporteService> logger,
            IAdministrador adminRepository,
            ICurrentUserService currentUserService,
            ReporteServiceValidator reporteServiceValidator
        ) : base(logger)
        {
            _reporteRepository = reporteRepository;
            _adminRepository = adminRepository;
            _currentUserService = currentUserService;
            _reporteServiceValidator = reporteServiceValidator;
        }

        public async Task<ServiceResult> CreateReporte(CreateReporteDTO dto)
        {
            return await ExecuteSafeAsync("crear el reporte", async () =>
            {
                var dtoValidation = _reporteServiceValidator.ValidateForCreate(dto);
                if (!dtoValidation.Success) return dtoValidation;

                var adminValidation = await _reporteServiceValidator.ValidateAdministradorExistente(dto.AdminId);
                if (!adminValidation.Success) return adminValidation;

                var admin = (Administrador)adminValidation.Data;
                var reporte = ReporteMapper.ToEntity(dto, admin.IdUsuario);

                var opResult = await _reporteRepository.Save(reporte);
                if (!opResult.Success) return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Reporte creado correctamente", reporte);
            });
        }

        public async Task<ServiceResult> GetReporte()
        {
            return await ExecuteSafeAsync("obtener los reportes", async () =>
            {
                var opResult = await _reporteRepository.GetAll();
                if (!opResult.Success || opResult.Data == null)
                    return new ServiceResult(false, "No se pudieron obtener los reportes");

                var reportes = ((List<Reporte>)opResult.Data)
                    .Select(ReporteMapper.ToDto)
                    .ToList();

                return new ServiceResult(true, "Reportes obtenidos correctamente", reportes);
            });
        }

        public async Task<ServiceResult> GetReporteById(int id)
        {
            return await ExecuteSafeAsync($"obtener el reporte con ID {id}", async () =>
            {
                var reporteValidation = await _reporteServiceValidator.ValidateReporteExistente(id);
                if (!reporteValidation.Success) return reporteValidation;

                var reporte = (Reporte)reporteValidation.Data;
                return new ServiceResult(true, "Reporte obtenido correctamente", ReporteMapper.ToDto(reporte));
            });
        }

        public async Task<ServiceResult> UpdateReporte(UpdateReporteDTO dto)
        {
            return await ExecuteSafeAsync($"actualizar el reporte con ID {dto.IdReporte}", async () =>
            {
                var dtoValidation = _reporteServiceValidator.ValidateForUpdate(dto);
                if (!dtoValidation.Success) return dtoValidation;

                var reporteValidation = await _reporteServiceValidator.ValidateReporteExistente(dto.IdReporte);
                if (!reporteValidation.Success) return reporteValidation;

                var adminValidation = await _reporteServiceValidator.ValidateAdministradorExistente(dto.AdminId);
                if (!adminValidation.Success) return adminValidation;

                var reporte = (Reporte)reporteValidation.Data;
                var usuarioModificacion = _currentUserService.GetUserId();
                if (usuarioModificacion == null)
                    return new ServiceResult(false, "Usuario no autenticado");

                ReporteMapper.MapToEntity(reporte, dto, usuarioModificacion.Value);

                var opResult = await _reporteRepository.Update(reporte);
                if (!opResult.Success) return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Reporte actualizado correctamente", reporte);
            });
        }

        public async Task<ServiceResult> RemoveReporte(DeleteReporteDTO dto)
        {
            return await ExecuteSafeAsync($"eliminar el reporte con ID {dto.IdReporte}", async () =>
            {
                var dtoValidation = _reporteServiceValidator.ValidateForDelete(dto);
                if (!dtoValidation.Success) return dtoValidation;

                var reporteValidation = await _reporteServiceValidator.ValidateReporteExistente(dto.IdReporte);
                if (!reporteValidation.Success) return reporteValidation;

                var reporte = (Reporte)reporteValidation.Data;
                var opResult = await _reporteRepository.Remove(reporte);
                if (!opResult.Success) return new ServiceResult(false, opResult.Message);

                return new ServiceResult(true, "Reporte eliminado correctamente");
            });
        }
    }
}

