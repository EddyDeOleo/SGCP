
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGCP.Application.Base.ServiceValidator.ModuloUsuarios;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services.ModuloUsuarios;
using SGCP.Infraestructure.Dependencies.DB_Context;
using SGCP.Infraestructure.Interfaces;
using SGCP.Infraestructure.Security;
using SGCP.Persistence.Repositories.ModuloUsuarios;

namespace SGCP.Ioc.Dependencies.ModuloUsuarios
{
    public static class UsuarioDependency
    {
        public static void AddUsuarioDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDBContextDependencies(configuration);


            services.AddScoped<ICliente, ClienteRepositoryEF>();
            services.AddTransient<IClienteService, ClienteService>();

            services.AddScoped<IAdministrador, AdministradorRepositoryEF>();
            services.AddTransient<IAdminService, AdminService>();


            services.AddScoped<AdminServiceValidator>();
            services.AddScoped<ClienteServiceValidator>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();


        }
    }
}
