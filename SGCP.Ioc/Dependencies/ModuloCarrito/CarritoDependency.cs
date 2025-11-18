using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGCP.Application.Base.ServiceValidator.ModuloCarrito;
using SGCP.Application.Interfaces.IServiceValidator.ModuloCarrito;
using SGCP.Application.Interfaces.ModuloCarrito;
using SGCP.Application.Interfaces.ModuloUsuarios;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services.ModuloCarrito;
using SGCP.Application.Services.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Infraestructure.Dependencies.DB_Context;
using SGCP.Infraestructure.Interfaces;
using SGCP.Infraestructure.Security;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityValidator.ModuloCarrito;
using SGCP.Persistence.Base.EntityValidator.ModuloProducto;
using SGCP.Persistence.Base.EntityValidator.ModuloUsuarios;
using SGCP.Persistence.Base.IEntityValidator;
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
            services.AddScoped<IEntityValidator<Carrito>, CarritoValidator>();
            services.AddScoped<ICarritoServiceValidator, CarritoServiceValidator>();
            services.AddScoped<ICarrito, CarritoRepositoryAdo>();
            services.AddTransient<ICarritoService, CarritoService>();
            services.AddScoped<ICarritoProducto, CarritoProductoRepositoryAdo>();
            services.AddScoped<IProducto, ProductoRepositoryAdo>();
            services.AddScoped<IEntityValidator<Producto>, ProductoValidator>();


            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddScoped<IAdministrador, AdministradorRepositoryEF>();


            services.AddScoped<IEntityValidator<Administrador>, AdministradorValidator>();

            services.AddScoped<IEntityValidator<Cliente>, ClienteValidator>();

            services.AddScoped<ICliente, ClienteRepositoryEF>();
            services.AddHttpContextAccessor();


        }
    }
}
