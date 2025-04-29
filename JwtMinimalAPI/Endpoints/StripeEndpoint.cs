using JwtMinimalAPI.StripeConfigs;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace JwtMinimalAPI.Endpoints
{
    public class StripeEndpoint(StripeModel model, CustomerService customerService, ProductService productService)
    {
        private readonly StripeModel model = model;
        private readonly CustomerService customerService = customerService;
        private readonly ProductService productService = productService;

        public void GetStripeEndpoints(WebApplication app)
        {
            app.MapPost("api/stripe/Pay", Pay);

            app.MapGet("api/stripe/GetProducts", async () =>
            {
                StripeConfiguration.ApiKey = model.SecretKey; // Get the secret key from the configuration
                var options = new ProductListOptions { Expand = ["data.default_price"] }; // Expand the default price
                var service = new ProductService();
                var products = await service.ListAsync(options);
                return Results.Ok(products); // Return the list of products
            });
            app.MapPost("api/stripe/CreateCustomer", async (StripeCustomer customerInfo) =>
            {
                StripeConfiguration.ApiKey = model.SecretKey;

                var customerOptions = new CustomerCreateOptions
                {
                    Email = customerInfo.Email,
                    Name = customerInfo.Name
                };

                var customer = await customerService.CreateAsync(customerOptions);

                return Results.Ok(new { customer });
            });
        }

        public async Task<IResult> Pay(string priceId)  // currently guest checkout
        {
            StripeConfiguration.ApiKey = model.SecretKey; // Get the secret key from the configuration

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId, // Get the price ID from the request
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://example.com/success", // fix Later    
                CancelUrl = "https://example.com/cancel" // fix Later
            };

            var service = new SessionService();

            Session session = service.Create(options);

            return Results.Ok(session.Url); // Return the session URL for redirection
        }
    }
}
