using JwtMinimalAPI.StripeConfigs;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace JwtMinimalAPI.Endpoints
{
    public static class StripeEndpoints
    {
        public static void GetStripeEndpoints(WebApplication app)
        {
            app.MapPost("api/stripe/Pay", Pay);
            app.MapGet("api/stripe/GetProducts", GetProducts);
            app.MapPost("api/stripe/CreateCustomer", CreateCustomer);
        }

        private static async Task<IResult> Pay(string priceId, IOptions<StripeModel> options)
        {
            var model = options.Value;
            StripeConfiguration.ApiKey = model.SecretKey;

            var sessionOptions = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://example.com/success",
                CancelUrl = "https://example.com/cancel"
            };
            //sessionOptions.Customer = "cus_SDfKUgt7GYAlgU";
            var service = new SessionService();
            Session session = service.Create(sessionOptions);

            return Results.Ok(session.Url);
        }

        private static async Task<IResult> GetProducts(IOptions<StripeModel> options)
        {
            var model = options.Value;
            StripeConfiguration.ApiKey = model.SecretKey;

            var productOptions = new ProductListOptions { Expand = ["data.default_price"] };
            var service = new ProductService();
            var products = await service.ListAsync(productOptions);

            return Results.Ok(products);
        }

        private static async Task<IResult> CreateCustomer(StripeCustomer customerInfo, CustomerService customerService, IOptions<StripeModel> options)
        {
            var model = options.Value;
            StripeConfiguration.ApiKey = model.SecretKey;

            var customerOptions = new CustomerCreateOptions
            {
                Email = customerInfo.Email,
                Name = customerInfo.Name
            };
            
            var customer = await customerService.CreateAsync(customerOptions);

            return Results.Ok(new { customer });
        }
    }
}