using JwtMinimalAPI.Services;
using JwtMinimalAPI.Services.ServiceInterfaces;
using Microsoft.IdentityModel.Tokens;

namespace JwtMinimalAPI.Extentions
{
    public static class ApplicationServicesExtentions
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            services.AddOpenApi();
            // Add services to the container.
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<PasswordService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ChatBotService>();
            services.AddAuthorizationBuilder().AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));

            return services;
        }
    }
}
