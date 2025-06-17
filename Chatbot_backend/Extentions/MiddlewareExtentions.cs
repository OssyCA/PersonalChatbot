using Chatbot_backend.Middlewere;
using Chatbot_backend.Middlewere;
using Scalar.AspNetCore;

namespace Chatbot_backend.Extentions
{
    public static class MiddlewareExtentions
    {
        public static WebApplication ConfigMiddleware(this WebApplication app)
        {
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }
            app.UseExceptionHandling();        // 1. Exception handling should be first
            app.UseHttpsRedirection();         // 2. HTTPS redirection early
            app.UseCors("AllowAllOrigins");    // 3. CORS headers before authentication
            app.UseAuthentication();           // 4. Authentication before refresh
            app.UseTokenRefresh();             // 5. Token refresh after authentication
            app.UseAuthorization();            // 6. Authorization after authentication
            app.UseRateLimiter();
            return app;
        }
    }
}
