using Chatbot_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_backend.Extentions
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
