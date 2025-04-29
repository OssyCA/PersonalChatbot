using JwtMinimalAPI.Services;
using JwtMinimalAPI.Services.ServiceInterfaces;
using JwtMinimalAPI.StripeConfigs;
using Microsoft.IdentityModel.Tokens;
using Stripe;

namespace JwtMinimalAPI.Extentions
{
    public static class ApplicationServicesExtentions
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>(); // Get the configuration
            services.AddOpenApi();
            // Add services to the container.
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<PasswordService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ChatBotService>();
            services.AddAuthorizationBuilder().AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));

            // stripe
            services.Configure<StripeModel>(configuration.GetSection("Stripe"));
            services.AddScoped<CustomerService>();
            services.AddScoped<ProductService>();
            

            return services;
        }
    }
}
