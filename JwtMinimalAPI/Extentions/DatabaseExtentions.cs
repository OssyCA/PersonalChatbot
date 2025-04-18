using JwtMinimalAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace JwtMinimalAPI.Extentions
{
    public static class DatabaseExtentions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MiniJwtDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("UserDatabase"));
            });

            return services;
        }
    }
}
