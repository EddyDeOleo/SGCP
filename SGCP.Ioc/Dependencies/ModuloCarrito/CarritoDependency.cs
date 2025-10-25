using Microsoft.Extensions.DependencyInjection;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Application.Services;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityValidator.ModuloCarrito;
using SGCP.Persistence.Base.EntityValidator.ModuloProducto;
using SGCP.Persistence.Repositories.ModuloCarrito;
using SGCP.Persistence.Repositories.ModuloProducto;

namespace SGCP.Ioc.Dependencies.ModuloCarrito
{
    public static class CarritoDependency
    {
        public static void AddCarritoDependencies(this IServiceCollection services)
        {
            services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            services.AddScoped<CarritoValidator>();
            services.AddScoped<ICarrito, CarritoRepositoryAdo>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddTransient<ICarritoService, CarritoService>();
            services.AddScoped<ICarritoProducto, CarritoProductoRepositoryAdo>();
            services.AddScoped<IProducto, ProductoRepositoryAdo>();
            services.AddScoped<ProductoValidator>();
        }
    }
}
