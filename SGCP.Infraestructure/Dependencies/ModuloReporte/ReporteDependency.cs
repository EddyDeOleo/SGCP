
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloReporte;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services;
using SGCP.Infraestructure.Dependencies.DB_Context;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityValidator.ModuloReporte;
using SGCP.Persistence.Repositories.ModuloReporte;
using SGCP.Persistence.Repositories.ModuloUsuarios;

namespace SGCP.Infraestructure.Dependencies.ModuloReporte
{
    public static class ReporteDependency
    {
        public static void AddReporteDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDBContextDependencies(configuration);


            services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            services.AddScoped<ReporteValidator>();
            services.AddScoped<IReporte, ReporteRepositoryAdo>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddTransient<IReporteService, ReporteService>();
            services.AddScoped<IAdministrador, AdministradorRepositoryEF>();

        }
    }
}
