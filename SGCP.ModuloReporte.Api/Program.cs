
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloReporte;
using SGCP.Application.Services;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityValidator.ModuloReporte;
using SGCP.Persistence.Repositories.ModuloReporte;

namespace SGCP.ModuloReporte.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            builder.Services.AddScoped<ReporteValidator>();
            builder.Services.AddScoped<IReporte, ReporteRepositoryAdo>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddTransient<IReporteService, ReporteService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
