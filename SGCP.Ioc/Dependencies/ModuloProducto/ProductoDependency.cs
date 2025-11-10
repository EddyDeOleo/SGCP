

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGCP.Application.Base.ServiceValidator.ModuloProducto;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services.ModuloProducto;
using SGCP.Application.Services.ModuloUsuarios;
using SGCP.Infraestructure.Dependencies.DB_Context;
using SGCP.Infraestructure.Interfaces;
using SGCP.Infraestructure.Security;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityValidator.ModuloProducto;
using SGCP.Persistence.Repositories.ModuloProducto;
using SGCP.Persistence.Repositories.ModuloUsuarios;

namespace SGCP.Ioc.Dependencies.ModuloProducto
{
    public static class ProductoDependency
    {
        public static void AddProductoDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDBContextDependencies(configuration);


            services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            services.AddScoped<ProductoValidator>();
            services.AddScoped<ProductoServiceValidator>();

            services.AddScoped<IProducto, ProductoRepositoryAdo>();
            services.AddTransient<IProductoService, ProductoService>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAdministrador, AdministradorRepositoryEF>();
            services.AddScoped<ICliente, ClienteRepositoryEF>();

            services.AddHttpContextAccessor();


        }
    }
}
