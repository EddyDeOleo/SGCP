
using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloReporte.Reporte;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloReporte;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeReporte;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Services
    {
        public sealed class ReporteService : IReporteService
        {
            private readonly IReporte _reporteRepository;
            private readonly ILogger<ReporteService> _logger;
            private readonly ISessionService _sessionService;
            private readonly IAdministrador _adminRepository;

        public ReporteService(IReporte reporteRepository, ILogger<ReporteService> logger, ISessionService sessionService, IAdministrador adminRepository)
            {
                _reporteRepository = reporteRepository;
                _logger = logger;
                _sessionService = sessionService;
                _adminRepository = adminRepository;
        }

        public async Task<ServiceResult> CreateReporte(CreateReporteDTO createReporteDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation("Iniciando la creación de un nuevo reporte");

            // Validación: admin existe
            var adminResult = await _adminRepository.GetEntityBy(createReporteDto.AdminId);
            if (!adminResult.Success)
            {
                result.Success = false;
                result.Message = "Administrador inválido";
                return result;
            }

            var adminExiste = (Administrador)adminResult.Data;

            // Validación: fecha obligatoria
            if (createReporteDto.FechaCreacion == default)
            {
                result.Success = false;
                result.Message = "La fecha del reporte es obligatoria";
                return result;
            }

            try
            {
                var reporte = new Reporte
                {
                    AdminId = adminExiste.IdUsuario, // se asegura que sea un admin real
                    FechaCreacion = createReporteDto.FechaCreacion,
                    TotalVentas = createReporteDto.TotalVentas,
                    TotalPedidos = createReporteDto.TotalPedidos
                };

                var opResult = await _reporteRepository.Save(reporte);
                if (!opResult.Success)
                {
                    result.Success = false;
                    result.Message = opResult.Message;
                    _logger.LogWarning("No se pudo guardar el reporte en la base de datos.");
                    return result;
                }

                result.Success = true;
                result.Message = "Reporte creado correctamente";
                result.Data = reporte;
                _logger.LogInformation($"Reporte creado exitosamente con ID: {reporte.IdReporte}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el reporte");
                result.Success = false;
                result.Message = "Ocurrió un error al crear el reporte";
            }

            return result;
        }

        public async Task<ServiceResult> GetReporte()
            {
                var result = new ServiceResult();
                _logger.LogInformation("Obteniendo todos los reportes");

                try
                {
                    var opResult = await _reporteRepository.GetAll();
                    if (!opResult.Success || opResult.Data == null)
                    {
                        result.Success = false;
                        result.Message = "No se pudieron obtener los reportes";
                        _logger.LogWarning("Error al obtener reportes");
                        return result;
                    }

                    var reportes = ((List<Reporte>)opResult.Data)
                        .Select(r => new ReporteGetDTO
                        {
                            IdReporte = r.IdReporte,
                            AdminId = r.AdminId,
                            TotalVentas = r.TotalVentas,
                            TotalPedidos = r.TotalPedidos,
                            FechaCreacion = r.FechaCreacion
                        }).ToList();

                    result.Success = true;
                    result.Data = reportes;
                    result.Message = "Reportes obtenidos correctamente";
                    _logger.LogInformation("Reportes obtenidos exitosamente");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener los reportes");
                    result.Success = false;
                    result.Message = "Ocurrió un error al obtener los reportes";
                }

                return result;
            }

            public async Task<ServiceResult> GetReporteById(int id)
            {
                var result = new ServiceResult();
                _logger.LogInformation("Obteniendo reporte con ID {ReporteId}", id);

                try
                {
                    var opResult = await _reporteRepository.GetEntityBy(id);
                    if (!opResult.Success || opResult.Data == null)
                    {
                        result.Success = false;
                        result.Message = "Reporte no encontrado";
                        _logger.LogWarning("Reporte con ID {ReporteId} no encontrado", id);
                        return result;
                    }

                    var reporte = (Reporte)opResult.Data;

                    var getReporteDto = new ReporteGetDTO
                    {
                        IdReporte = reporte.IdReporte,
                        AdminId = reporte.AdminId,
                        TotalVentas = reporte.TotalVentas,
                        TotalPedidos = reporte.TotalPedidos,
                        FechaCreacion = reporte.FechaCreacion
                    };

                    result.Success = true;
                    result.Data = getReporteDto;
                    result.Message = "Reporte obtenido correctamente";
                    _logger.LogInformation("Reporte con ID {ReporteId} obtenido exitosamente", id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener el reporte con ID {ReporteId}", id);
                    result.Success = false;
                    result.Message = "Ocurrió un error al obtener el reporte";
                }

                return result;
            }

            public async Task<ServiceResult> UpdateReporte(UpdateReporteDTO updateReporteDto)
            {
                var result = new ServiceResult();
                _logger.LogInformation($"Iniciando actualización del reporte con ID: {updateReporteDto.IdReporte}");

            /*
                if (_sessionService.AdminIdLogueado == null)
                {
                    result.Success = false;
                    result.Message = "El administrador debe iniciar sesión para actualizar reportes.";
                    return result;
                }
            */
                try
                {
                    var existingResult = await _reporteRepository.GetEntityBy(updateReporteDto.IdReporte);
                    if (!existingResult.Success || existingResult.Data == null)
                    {
                        result.Success = false;
                        result.Message = "Reporte no encontrado";
                        _logger.LogWarning($"Reporte con ID {updateReporteDto.IdReporte} no encontrado");
                        return result;
                    }

                    var reporteExistente = (Reporte)existingResult.Data;

                    reporteExistente.AdminId = updateReporteDto.AdminId;
                    reporteExistente.FechaCreacion = updateReporteDto.FechaCreacion;
                    reporteExistente.TotalVentas = updateReporteDto.TotalVentas;
                    reporteExistente.TotalPedidos = updateReporteDto.TotalPedidos;

                    var opResult = await _reporteRepository.Update(reporteExistente);
                    if (!opResult.Success)
                    {
                        result.Success = false;
                        result.Message = opResult.Message;
                        _logger.LogWarning($"No se pudo actualizar el reporte con ID {updateReporteDto.IdReporte}");
                        return result;
                    }

                    result.Success = true;
                    result.Message = "Reporte actualizado correctamente";
                    result.Data = reporteExistente;
                    _logger.LogInformation($"Reporte con ID {updateReporteDto.IdReporte} actualizado exitosamente");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al actualizar el reporte con ID {updateReporteDto.IdReporte}");
                    result.Success = false;
                    result.Message = "Ocurrió un error al actualizar el reporte";
                }

                return result;
            }

            public async Task<ServiceResult> RemoveReporte(DeleteReporteDTO deleteReporteDto)
            {
                var result = new ServiceResult();
                _logger.LogInformation($"Iniciando eliminación del reporte con ID: {deleteReporteDto.IdReporte}");

            /*
                if (_sessionService.AdminIdLogueado == null)
                {
                    result.Success = false;
                    result.Message = "El administrador debe iniciar sesión para eliminar reportes.";
                    return result;
                }
            */
                try
                {
                    var existingResult = await _reporteRepository.GetEntityBy(deleteReporteDto.IdReporte);
                    if (!existingResult.Success || existingResult.Data == null)
                    {
                        result.Success = false;
                        result.Message = "Reporte no encontrado";
                        _logger.LogWarning($"Reporte con ID {deleteReporteDto.IdReporte} no encontrado");
                        return result;
                    }

                    var reporteExistente = (Reporte)existingResult.Data;

                    var opResult = await _reporteRepository.Remove(reporteExistente);
                    if (!opResult.Success)
                    {
                        result.Success = false;
                        result.Message = opResult.Message;
                        _logger.LogWarning($"No se pudo eliminar el reporte con ID {deleteReporteDto.IdReporte}");
                        return result;
                    }

                    result.Success = true;
                    result.Message = "Reporte eliminado correctamente";
                    _logger.LogInformation($"Reporte con ID {deleteReporteDto.IdReporte} eliminado exitosamente");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al eliminar el reporte con ID {deleteReporteDto.IdReporte}");
                    result.Success = false;
                    result.Message = "Ocurrió un error al eliminar el reporte";
                }

                return result;
            }
        }
}
