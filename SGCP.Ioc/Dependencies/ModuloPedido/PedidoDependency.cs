

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGCP.Application.Base.ServiceValidator.ModuloCarrito;
using SGCP.Application.Base.ServiceValidator.ModuloPedido;
using SGCP.Application.Base.ServiceValidator.ModuloUsuarios;
using SGCP.Application.Interfaces.IServiceValidator.ModuloCarrito;
using SGCP.Application.Interfaces.IServiceValidator.ModuloPedido;
using SGCP.Application.Interfaces.IServiceValidator.ModuloUsuarios;
using SGCP.Application.Interfaces.ModuloCarrito;
using SGCP.Application.Interfaces.ModuloPedido;
using SGCP.Application.Interfaces.ModuloUsuarios;
using SGCP.Application.Repositories.ModuloCarrito;
using SGCP.Application.Repositories.ModuloPedido;
using SGCP.Application.Repositories.ModuloProducto;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services.ModuloCarrito;
using SGCP.Application.Services.ModuloPedido;
using SGCP.Application.Services.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Infraestructure.Dependencies.DB_Context;
using SGCP.Infraestructure.Interfaces;
using SGCP.Infraestructure.Security;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.EntityValidator.ModuloCarrito;
using SGCP.Persistence.Base.EntityValidator.ModuloPedido;
using SGCP.Persistence.Base.EntityValidator.ModuloProducto;
using SGCP.Persistence.Base.EntityValidator.ModuloUsuarios;
using SGCP.Persistence.Base.IEntityValidator;
using SGCP.Persistence.Repositories.ModuloCarrito;
using SGCP.Persistence.Repositories.ModuloPedido;
using SGCP.Persistence.Repositories.ModuloProducto;
using SGCP.Persistence.Repositories.ModuloUsuarios;

namespace SGCP.Ioc.Dependencies.ModuloPedido
{
    public static class PedidoDependency
    {
        public static void AddPedidoDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDBContextDependencies(configuration);

            services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            services.AddScoped<IEntityValidator<Pedido>, PedidoValidator>();
            services.AddScoped<IPedido, PedidoRepositoryAdo>();
            services.AddTransient<IPedidoService, PedidoService>();
            services.AddScoped<IEntityValidator<Carrito>, CarritoValidator>();
            services.AddScoped<IPedidoServiceValidator,PedidoServiceValidator>();
            services.AddScoped<ICarrito, CarritoRepositoryAdo>();
            services.AddTransient<ICarritoService, CarritoService>();

            services.AddScoped<ICarritoProducto, CarritoProductoRepositoryAdo>();
            services.AddScoped<IProducto, ProductoRepositoryAdo>();
            services.AddScoped<IEntityValidator<Producto>, ProductoValidator>();

            services.AddScoped<IPedidoProducto, PedidoProductoRepositoryAdo>();

            services.AddScoped<ICliente, ClienteRepositoryEF>();
            services.AddTransient<IClienteService, ClienteService>();


            services.AddScoped<IEntityValidator<Administrador>, AdministradorValidator>();

            services.AddScoped<IEntityValidator<Cliente>, ClienteValidator>();



            services.AddScoped<IAdministrador, AdministradorRepositoryEF>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddHttpContextAccessor();

            services.AddScoped<IClienteServiceValidator, ClienteServiceValidator>();

            services.AddScoped<ICarritoServiceValidator, CarritoServiceValidator>();




        }
    }
}
