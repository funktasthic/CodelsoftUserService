using System.Text;
using Api.Services;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Api.Data;
using UserService.Api.Exceptions;
using UserService.Api.Repositories;
using UserService.Api.Repositories.Interfaces;

namespace UserService.Api.Extensions
{
    public static class AppServiceExtensions
    {
        public static void AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            AddAutoMapper(services);
            AddServices(services);
            AddDbContext(services, config);
            AddUnitOfWork(services);
            AddAuthentication(services, config);
            AddHttpContextAccesor(services);
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IMapperService, MapperService>();
            services.AddScoped<IAuthService, AuthService>();
        }


        private static void AddDbContext(IServiceCollection services, IConfiguration config)
        {
            var ConnectionString = config.GetConnectionString("Database");

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseNpgsql(ConnectionString);
            });
        }

        private static void AddUnitOfWork(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void AddAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
        }

        private static IServiceCollection AddAuthentication(
            IServiceCollection services,
            IConfiguration config
        )
        {
            var jwtSecret = config.GetSection("JwtSettings").GetValue<string>("Secret");

            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidJwtException("JWT_SECRET not present in appsettings.json");
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(jwtSecret)
                        ),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });
            return services;
        }

        private static void AddHttpContextAccesor(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
        }
    }
}
