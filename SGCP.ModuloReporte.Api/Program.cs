
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SGCP.Ioc.Dependencies.ModuloReporte;
using System.Text;

namespace SGCP.ModuloReporte.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // ==========================
            // LOGGING CONFIG
            // ==========================
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);

            // ==========================
            // DEPENDENCIAS
            // ==========================
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddReporteDependencies(builder.Configuration);

            // ==========================
            // JWT CONFIGURATION
            // ==========================
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            Console.WriteLine("========================================");
            Console.WriteLine("JWT CONFIGURATION (ModuloReporte):");
            Console.WriteLine($"SecretKey Length: {secretKey?.Length ?? 0}");
            Console.WriteLine($"Issuer: '{issuer}'");
            Console.WriteLine($"Audience: '{audience}'");
            Console.WriteLine("========================================");

            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("Error: JWT SecretKey is NULL or empty!");

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                            if (!string.IsNullOrEmpty(token))
                                Console.WriteLine($"[JWT] Token received (first 30): {token.Substring(0, Math.Min(30, token.Length))}...");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var userId = context.Principal?.FindFirst("UserId")?.Value;
                            Console.WriteLine($"[JWT] ✅ Token validated for UserId={userId}");
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"[JWT] ❌ Auth failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            Console.WriteLine($"[JWT] ⚠️ Challenge: {context.Error} - {context.ErrorDescription}");
                            return Task.CompletedTask;
                        }
                    };
                });

            // ==========================
            // SWAGGER CONFIG (con JWT)
            // ==========================
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SGCP Reporte API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Autenticación JWT usando Bearer. Ejemplo: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // ==========================
            // MIDDLEWARE ORDER
            // ==========================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();  
            app.UseAuthorization();

            app.MapControllers();

            Console.WriteLine("========================================");
            Console.WriteLine("✅ Reporte API is running and secured with JWT");
            Console.WriteLine("========================================");

            app.Run();
        }
    }
}

