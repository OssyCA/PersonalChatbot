using Chatbot_backend.Helpers.EmailConfig;

namespace Chatbot_backend.Extentions
{
    public static class MailExtentions
    {
        public static IServiceCollection GetMailConfig(this IServiceCollection services, IConfiguration configuration)
        {

            // Get the GmailOptions section from the appsettings.json file
            services.Configure<GmailOptions>(
                configuration.GetSection(GmailOptions.GmailOptionsKey));

            return services;
        }
    }
}
