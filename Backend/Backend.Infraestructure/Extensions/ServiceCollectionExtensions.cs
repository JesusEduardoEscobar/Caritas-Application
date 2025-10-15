using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Backend.Infraestructure.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Backend.Infrastructure.Extensions
{   
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<NeonTechDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }
    }
}
