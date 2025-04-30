using JwtMinimalAPI.StripeConfigs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace JwtMinimalAPI.Endpoints
{
    public static class StripeEndpoints
    {
        public static void MapEndpoints(WebApplication app)
        {
            app.MapPost("api/stripe/pay", Pay);
            app.MapGet("api/stripe/products", GetProducts);
            app.MapPost("api/stripe/customers", CreateCustomer);
        }

        private static async Task<IResult> Pay([FromQuery] string priceId, IOptions<StripeModel> options)
        {
            if (string.IsNullOrEmpty(priceId))
                return Results.BadRequest("Missing priceId parameter");

            StripeConfiguration.ApiKey = options.Value.SecretKey;

            try
            {
                // Retrieve the price from Stripe
                var priceService = new PriceService();
                var price = await priceService.GetAsync(priceId);

                // Determine mode based on whether it's a subscription or one-time payment
                var isRecurring = price.Recurring != null;

                var sessionOptions = new SessionCreateOptions
                {
                    Mode = isRecurring ? "subscription" : "payment",
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            Price = priceId,
                            Quantity = 1,
                        },
                    },
                    SuccessUrl = "https://example.com/success",
                    CancelUrl = "https://example.com/cancel"
                };

                var sessionService = new SessionService();
                var session = await sessionService.CreateAsync(sessionOptions);

                return Results.Ok(session.Url);
            }
            catch (StripeException ex)
            {
                // Extract Stripe-specific error info if available
                var stripeError = ex.StripeError;
                var errorMessage = stripeError != null
                    ? $"Stripe error: {stripeError.Code ?? "unknown"} – {stripeError.Message}"
                    : ex.Message;

                // Log the exception for debugging
                Console.Error.WriteLine($"StripeException: {ex}");
                if (stripeError != null)
                    Console.Error.WriteLine($"StripeError details: {stripeError}");

                return Results.Problem(
                    detail: errorMessage,
                    statusCode: stripeError?.Type == "invalid_request_error" ? 400 : 500);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error: {ex}");
                return Results.Problem(
                    detail: "An unexpected error occurred while processing your payment request.",
                    statusCode: 500);
            }
        }
        private static async Task<IResult> GetProducts(IOptions<StripeModel> options)
        {
            StripeConfiguration.ApiKey = options.Value.SecretKey;

            try
            {
                var productOptions = new ProductListOptions
                {
                    Active = true,
                    Expand = ["data.default_price"]
                };

                var service = new ProductService();
                var products = await service.ListAsync(productOptions);

                return Results.Ok(products);
            }
            catch (StripeException ex)
            {
                Console.Error.WriteLine($"StripeException in GetProducts: {ex}");
                return Results.Problem(
                    detail: $"Error retrieving products: {ex.Message}",
                    statusCode: 500);
            }
        }
        private static async Task<IResult> CreateCustomer([FromBody] StripeCustomer customerInfo, [FromServices] CustomerService customerService, IOptions<StripeModel> options)
        {
            if (string.IsNullOrEmpty(customerInfo.Email))
                return Results.BadRequest("Customer email is required");

            StripeConfiguration.ApiKey = options.Value.SecretKey;

            try
            {
                var customerOptions = new CustomerCreateOptions
                {
                    Email = customerInfo.Email,
                    Name = customerInfo.Name
                };

                var customer = await customerService.CreateAsync(customerOptions);
                return Results.Ok(new { CustomerId = customer.Id });
            }
            catch (StripeException ex)
            {
                Console.Error.WriteLine($"StripeException in CreateCustomer: {ex}");
                return Results.Problem(
                    detail: $"Error creating customer: {ex.Message}",
                    statusCode: 500);
            }
        }
    }
}