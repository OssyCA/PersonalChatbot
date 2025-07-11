using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Chatbot_backend.Services
{
    public class ChatBotService
    {
        private readonly string? apiKey;
        private readonly string? endpoint;
        private readonly HttpClient httpClient;

        public ChatBotService(IConfiguration configuration, HttpClient? httpClient = null)
        {
            // Validate configuration
            apiKey = configuration["ChatBotApiKey"] ??
                throw new ArgumentNullException(nameof(configuration), "ChatBotApiKey configuration is missing");

            // Create endpoint URL
            //2.5 model
            //endpoint = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={apiKey}";

            // Free model
            endpoint = $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={apiKey}";



            // Use injected HttpClient or create a new one
            this.httpClient = httpClient ?? new HttpClient();
        }

        public async Task<string> GetResponse(string userMessage)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                return "User message cannot be empty.";
            }

            try
            {
                // Prepare the request data
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
                using HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send the request to the API
                HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

                // Read the API response
                string responseString = await response.Content.ReadAsStringAsync();

                // Handle unsuccessful status code
                if (!response.IsSuccessStatusCode)
                {
                    return $"Error: {response.StatusCode} - {responseString}";
                }

                // Parse response using JObject for safer navigation
                JObject responseObj = JObject.Parse(responseString);

                // Navigate safely through the JSON structure
                if (responseObj["candidates"] is JArray candidates && candidates.Count > 0)
                {
                    JToken? firstCandidate = candidates[0];
                    JToken? _content = firstCandidate?["content"];

                    if (_content?["parts"] is JArray parts && parts.Count > 0)
                    {
                        string? text = parts[0]?["text"]?.ToString();
                        return !string.IsNullOrEmpty(text)
                            ? text
                            : "AI response was empty.";
                    }
                }

                return "Could not extract response from API result. Check the response format.";
            }
            catch (HttpRequestException ex)
            {
                return $"Network error: {ex.Message}";
            }
            catch (JsonException ex)
            {
                return $"Error parsing the API response: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Unexpected error: {ex.Message}";
            }
        }
    }
}