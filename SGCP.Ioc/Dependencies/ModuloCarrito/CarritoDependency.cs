using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGCP.Application.Base.ServiceValidator.ModuloCarrito;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services.ModuloCarrito;
using SGCP.Application.Services.ModuloUsuarios;
using SGCP.Infraestructure.Dependencies.DB_Context;
using SGCP.Infraestructure.Interfaces;
using SGCP.Infraestructure.Security;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityValidator.ModuloCarrito;
using SGCP.Persistence.Base.EntityValidator.ModuloProducto;
using SGCP.Persistence.Repositories.ModuloCarrito;
using SGCP.Persistence.Repositories.ModuloProducto;
using SGCP.Persistence.Repositories.ModuloUsuarios;

namespace SGCP.Ioc.Dependencies.ModuloCarrito
{
    public static class CarritoDependency
    {
        public static void AddCarritoDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDBContextDependencies(configuration);

            services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            services.AddScoped<CarritoValidator>();
            services.AddScoped<CarritoServiceValidator>();
            services.AddScoped<ICarrito, CarritoRepositoryAdo>();
            services.AddTransient<ICarritoService, CarritoService>();
            services.AddScoped<ICarritoProducto, CarritoProductoRepositoryAdo>();
            services.AddScoped<IProducto, ProductoRepositoryAdo>();
            services.AddScoped<ProductoValidator>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddScoped<IAdministrador, AdministradorRepositoryEF>();

            services.AddScoped<ICliente, ClienteRepositoryEF>();
            services.AddHttpContextAccessor();


        }
    }
}
