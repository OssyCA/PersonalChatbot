using JwtMinimalAPI.Services;

namespace JwtMinimalAPI.Endpoints
{
    public static class AdminEndpoints
    {
        public static void GetAdminEndpoints(WebApplication app)
        {
            app.MapGet("api/GetUsers", GetAllUsers).RequireAuthorization("AdminPolicy");
        }

        private static async Task<IResult> GetAllUsers(IAdminService service)
        {
            var users = await service.GetUsers();

            if (users is not null)
            {
                return Results.Ok(users);
            }
            return Results.NotFound("No users");
        }
    }
}
