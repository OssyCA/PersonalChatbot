using Newtonsoft.Json;
using System.Text;

namespace JwtMinimalAPI.Services
{
    public class ChatBotService
    {
        private readonly string apiKey;
        private readonly string endpoint;

        public ChatBotService(IConfiguration configuration)
        {
            apiKey = configuration["ChatBotApiKey"];
            endpoint = $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={apiKey}";
        }

        public async Task<string> GetResponse(string userMessage)
        {
            using HttpClient client = new();

            // Prepare the request data in an anonymous object
            var requestData = new
            {
                contents = new[]
                {
                        new
                        {
                            parts = new[] { new { text = userMessage } }
                        }
                }
            };

            string json = JsonConvert.SerializeObject(requestData);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the request to the API
            HttpResponseMessage response = await client.PostAsync(endpoint, content);

            // Read the API response
            string responseString = await response.Content.ReadAsStringAsync();

            // Write the API response to the console
            // Console.WriteLine("API Response: " + responseString);

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode} - {responseString}";
            }

            try
            {
                // dynamic to handle the response data without creating a class
                dynamic responseData = JsonConvert.DeserializeObject(responseString);

                if (responseData?.candidates != null && responseData.candidates.Count > 0)
                {
                    // Return the first candidate from the API response
                    return responseData.candidates[0].content.parts[0].text.ToString();
                }
                else
                {
                    return "Ai could not generate a response. Check the API response.";
                }
            }
            catch (Exception ex)
            {
                return $"wrong parsing of the API response: {ex.Message}";
            }


        }
    }
}
