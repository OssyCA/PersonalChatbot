
using Chatbot_backend.Data;
using Chatbot_backend.DTO;
using Chatbot_backend.Endpoints;
using Chatbot_backend.Extentions;
using Chatbot_backend.Helpers.EmailConfig;
using Chatbot_backend.Middlewere;
using Chatbot_backend.Models;
using Chatbot_backend.Services;
using Chatbot_backend.Services.ServiceInterfaces;
using Chatbot_backend.StripeConfigs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

namespace Chatbot_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Advanced Logging
            builder.Host.UseSerilog((ctx, lc) => lc
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

            // Add services to the container.
            builder.Services
                .AddDatabaseServices(builder.Configuration)    // Add DbContext and Identity
                .AddCorsPolicy()                                // Add CORS policy
                .AuthenticationService(builder.Configuration)   // Add JWT Authentication
                .GetMailConfig(builder.Configuration)           // Add Email Configuration
                .AddRateLimit()                                 // Add Rate Limiting
                .ApplicationServices();                         // Add Application Services


            var app = builder.Build(); // Build the application


            app.ConfigMiddleware().MapEndpoints(); // returns WebApplication


            app.Run();
        }
    }
}
