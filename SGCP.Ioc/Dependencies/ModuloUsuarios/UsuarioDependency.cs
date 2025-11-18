
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGCP.Application.Base.ServiceValidator.ModuloUsuarios;
using SGCP.Application.Interfaces.IServiceValidator.ModuloUsuarios;
using SGCP.Application.Interfaces.ModuloUsuarios;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Infraestructure.Dependencies.DB_Context;
using SGCP.Infraestructure.Interfaces;
using SGCP.Infraestructure.Security;
using SGCP.Persistence.Base.EntityValidator.ModuloProducto;
using SGCP.Persistence.Base.EntityValidator.ModuloUsuarios;
using SGCP.Persistence.Base.IEntityValidator;
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

            services.AddScoped<IEntityValidator<Administrador>, AdministradorValidator>();

            services.AddScoped<IEntityValidator<Cliente>, ClienteValidator>();



            services.AddScoped<IAdministrador, AdministradorRepositoryEF>();
            services.AddTransient<IAdminService, AdminService>();


            services.AddScoped<IAdminServiceValidator, AdminServiceValidator>();
            services.AddScoped<IClienteServiceValidator, ClienteServiceValidator>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();


        }
    }
}
