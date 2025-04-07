
using JwtMinimalAPI.Data;
using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Endpoints;
using JwtMinimalAPI.Helpers.EmailConfig;
using JwtMinimalAPI.Middlewere;
using JwtMinimalAPI.Models;
using JwtMinimalAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;

namespace JwtMinimalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Services

            // Add services to the container.
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy => // Add a policy to the options
                {
                    policy.AllowAnyOrigin(); // Allow any origin
                    policy.AllowAnyMethod(); // Allow any method
                    policy.AllowAnyHeader(); // Allow any header

                });
            });
            builder.Services.AddDbContext<MiniJwtDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase"));
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Appsettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Appsettings:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Appsettings:Token")!)),
                    ValidateIssuerSigningKey = true
                };
            });

            // Get the GmailOptions section from the appsettings.json file
            builder.Services.Configure<GmailOptions>(
                builder.Configuration.GetSection(GmailOptions.GmailOptionsKey));


            builder.Services
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<ChatBotService>()
                .AddScoped<IMailService, GmailSerivce>();

            builder.Services.AddRateLimiter(options => 
            {
                // Add a policy to the options with a partition key to rate limit the login endpoint per IP address
                options.AddPolicy("login", httpcontext => RateLimitPartition.GetFixedWindowLimiter( // get the rate limiter with a fixed window
                    partitionKey: httpcontext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN", // Use the IP address as the partition key
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true, // Enable auto-replenishment so that the rate limiter will automatically replenish the limit
                        PermitLimit = 1, // Set the limit to 5 requests
                        QueueLimit = 0, // Set the queue limit to 0, get a 429(to many request) response when the limit is reached
                        Window = TimeSpan.FromMinutes(100) // Set the window to 10 minutes
                    }));
            });

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            #endregion

            #region Builder
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }
            app.UseAuthentication();
            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS

           

            app.UseExceptionHandling(); // Use the exception handling middleware
            app.UseAuthorization();

            #endregion

            //TestingPoints.HandleUsers(app);
            FrontendEndpoints.HandleUser(app);
            FrontendEndpoints.HandleChatBot(app);
            app.Run();
        }
    }
}
