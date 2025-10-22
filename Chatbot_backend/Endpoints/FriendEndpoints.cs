using Chatbot_backend.Models;
using Chatbot_backend.Services;

namespace Chatbot_backend.Endpoints
{
    public static class FriendEndpoints
    {
        public static void MapFriendEndpoints(WebApplication app)
        {
            app.MapGet("/api/get-user/{name}", FindUser);
        }
        private static async Task<IResult> FindUser(FriendsService service, string name)
        {
            var user = await service.FindUser(name);

            if (user is not null)
            {
                return Results.Ok(user);
            }
            return Results.NotFound("No users");
        }


    }
}
