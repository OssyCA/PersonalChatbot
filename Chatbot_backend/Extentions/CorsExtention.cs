namespace Chatbot_backend.Extentions
{
    public static class CorsExtention
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:5174") // Update with your React app URL
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); // This is important for cookies
                });
            });
            return services;
        }
    }
}
