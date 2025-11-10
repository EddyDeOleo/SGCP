
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGCP.Application.Base.ServiceValidator.ModuloReporte;
using SGCP.Application.Interfaces.IServiceValidator.ModuloReporte;
using SGCP.Application.Interfaces.ModuloReporte;
using SGCP.Application.Interfaces.ModuloUsuarios;
using SGCP.Application.Repositories.ModuloReporte;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services.ModuloReporte;
using SGCP.Application.Services.ModuloUsuarios;
using SGCP.Infraestructure.Dependencies.DB_Context;
using SGCP.Infraestructure.Interfaces;
using SGCP.Infraestructure.Security;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityValidator.ModuloReporte;
using SGCP.Persistence.Repositories.ModuloReporte;
using SGCP.Persistence.Repositories.ModuloUsuarios;

namespace SGCP.Ioc.Dependencies.ModuloReporte
{
    public static class ReporteDependency
    {
        public static void AddReporteDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDBContextDependencies(configuration);

            services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            services.AddScoped<ReporteValidator>();
            services.AddScoped<IReporte, ReporteRepositoryAdo>();
            services.AddTransient<IReporteService, ReporteService>();
            services.AddScoped<IAdministrador, AdministradorRepositoryEF>();

            services.AddScoped<ICliente, ClienteRepositoryEF>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddScoped<IReporteServiceValidator, ReporteServiceValidator>();

            services.AddHttpContextAccessor();






        }
    }
}
