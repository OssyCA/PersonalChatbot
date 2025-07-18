﻿using Chatbot_backend.Endpoints;

namespace Chatbot_backend.Extentions
{
    public static class EndpointExtentions
    {
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            AdminEndpoints.GetAdminEndpoints(app);
            UserEndpoints.GetUserEndpoints(app);
            ChatBotEndpoints.GetChatBotEndpoints(app);
            PasswordEndpoints.GetPasswordEndpoints(app);

            // Register Stripe endpoints
            StripeEndpoints.MapEndpoints(app);


            return app;
        }
    }
}
