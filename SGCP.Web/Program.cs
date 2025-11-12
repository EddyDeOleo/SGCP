using SGCP.Ioc.Dependencies.ModuloPedido;
using SGCP.Ioc.Dependencies.ModuloProducto;
using SGCP.Ioc.Dependencies.ModuloReporte;
using SGCP.Ioc.Dependencies.ModuloUsuarios;
using SGCP.Ioc.Dependencies.ServiceCollectionExtensions;
using SGCP.Web.Filters;

namespace SGCP.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<SessionAuthorizationFilter>();
            });

            builder.Services.AddDistributedMemoryCache(); 
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); 
                options.Cookie.HttpOnly = true; 
                options.Cookie.IsEssential = true; 
                options.Cookie.Name = ".SGCP.Session"; 
            });

            builder.Services.AddUsuarioDependencies(builder.Configuration);
            builder.Services.AddProductoDependencies(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddReporteDependencies(builder.Configuration);
            builder.Services.AddPedidoDependencies(builder.Configuration);


            // PONER LAS DEPENDENCIAS DE LOS MODULOS AQUI Y MODIFICAR EL APPSETTINGS.JSON
            // PONER QUE MULTIPLES PROYECTOS INICIEN

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

          
            app.UseAuthentication();
            app.UseSession(); 

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}