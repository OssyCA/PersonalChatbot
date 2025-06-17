using Chatbot_backend.Endpoints;
using Chatbot_backend.Services;
using Chatbot_backend.Services.ServiceInterfaces;
using Chatbot_backend.StripeConfigs;
using Microsoft.IdentityModel.Tokens;
using Stripe;

namespace Chatbot_backend.Extentions
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
            services.AddScoped<Stripe.Checkout.SessionService>();

            return services;
        }
    }
}
