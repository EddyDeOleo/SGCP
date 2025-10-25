
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloPedido;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Application.Services;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityValidator.ModuloCarrito;
using SGCP.Persistence.Base.EntityValidator.ModuloPedido;
using SGCP.Persistence.Base.EntityValidator.ModuloProducto;
using SGCP.Persistence.Repositories.ModuloCarrito;
using SGCP.Persistence.Repositories.ModuloPedido;
using SGCP.Persistence.Repositories.ModuloProducto;

namespace SGCP.ModuloPedido.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            builder.Services.AddScoped<PedidoValidator>();
            builder.Services.AddScoped<IPedido, PedidoRepositoryAdo>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddTransient<IPedidoService, PedidoService>();
            builder.Services.AddScoped<CarritoValidator>();
            builder.Services.AddScoped<ICarrito, CarritoRepositoryAdo>();
            builder.Services.AddTransient<ICarritoService, CarritoService>();

            builder.Services.AddScoped<ICarritoProducto, CarritoProductoRepositoryAdo>();
            builder.Services.AddScoped<IProducto, ProductoRepositoryAdo>();
            builder.Services.AddScoped<ProductoValidator>();




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
