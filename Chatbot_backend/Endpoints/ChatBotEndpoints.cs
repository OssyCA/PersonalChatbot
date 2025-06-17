using Chatbot_backend.Services;

namespace Chatbot_backend.Endpoints
{
    public static class ChatBotEndpoints
    {
        public static void GetChatBotEndpoints(WebApplication app)
        {
            app.MapPost("/InputMessage/{inputMessage}", GetChatBotResponse).RequireAuthorization();
        }
        private static async Task<IResult> GetChatBotResponse(ChatBotService service, string inputMessage)
        {
            var chatResponse = await service.GetResponse(inputMessage);
            return Results.Ok(chatResponse);
        }
    }
}
