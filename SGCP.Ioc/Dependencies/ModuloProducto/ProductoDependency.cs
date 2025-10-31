

using Microsoft.Extensions.DependencyInjection;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Application.Services;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityValidator.ModuloProducto;
using SGCP.Persistence.Repositories.ModuloProducto;

namespace SGCP.Ioc.Dependencies.ModuloProducto
{
    public static class ProductoDependency
    {
        public static void AddProductoDependencies(this IServiceCollection services)
        {
            services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            services.AddScoped<ProductoValidator>();
            services.AddScoped<IProducto, ProductoRepositoryAdo>();
            services.AddTransient<IProductoService, ProductoService>();
        }
    }
}
