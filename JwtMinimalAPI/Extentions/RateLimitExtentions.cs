using System.Threading.RateLimiting;

namespace JwtMinimalAPI.Extentions
{
    public static class RateLimitExtentions
    {
        public static IServiceCollection AddRateLimit(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Add a policy to the options with a partition key to rate limit the login endpoint per IP address
                options.AddPolicy("login", httpcontext => RateLimitPartition.GetFixedWindowLimiter( // get the rate limiter with a fixed window
                    partitionKey: httpcontext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN", // Use the IP address as the partition key
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true, // Enable auto-replenishment so that the rate limiter will automatically replenish the limit
                        PermitLimit = 5, // Set the limit to 5 requests
                        QueueLimit = 0, // Set the queue limit to 0, get a 429(to many request) response when the limit is reached
                        Window = TimeSpan.FromMinutes(10) // Set the window to 10 minutes
                    }));
            });
            return services;
        }
    }
}
