
using JwtMinimalAPI.Data;
using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Endpoints;
using JwtMinimalAPI.Extentions;
using JwtMinimalAPI.Helpers.EmailConfig;
using JwtMinimalAPI.Middlewere;
using JwtMinimalAPI.Models;
using JwtMinimalAPI.Services;
using JwtMinimalAPI.Services.ServiceInterfaces;
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

namespace JwtMinimalAPI
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
