
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SGCP.Infraestructure.Dependencies.DB_Context
{
    public static class DB_ContextDependency
    {
        public static void AddDBContextDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SGCPDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
