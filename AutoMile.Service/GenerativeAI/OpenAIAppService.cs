using AutoMile.Domain.GenerativeAI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AutoMile.Service.GenerativeAI
{
    public class OpenAIAppService : IOpenAIAppService
    {
        private const string OpenAIEndpoint = "https://api.openai.com/v1/chat/completions";
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OpenAIAppService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            InitializeHttpClient();
        }

        private void InitializeHttpClient()
        {
            var apiKey = _configuration["ChatGPT:Key"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<List<string>> CreateUserStoriesFromGPT(string content)
        {
            var requestContent = CreateRequestContent(content, "You are an AI chatbot assisting with project management. Please generate a user story based on the following information:");
            var assistantResponse = await GetAssistantResponse(requestContent);

            return ParseUserStories(assistantResponse);
        }

        public async Task<string> GenerateAcceptanceCriteriaFromUserStory(string userStory)
        {
            var requestContent = CreateRequestContent(userStory, "You are an AI chatbot assisting with project management. Please generate acceptance criteria based on the following user story:");
            var assistantResponse = await GetAssistantResponse(requestContent);

            return assistantResponse;
        }

        private async Task<string> GetAssistantResponse(object requestContent)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(OpenAIEndpoint, jsonContent);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            var jsonObject = JObject.Parse(responseBody);
            var assistantResponse = jsonObject["choices"][0]["message"]["content"].ToString();

            return assistantResponse;
        }

        private object CreateRequestContent(string content, string systemMessage)
        {
            return new
            {
                model = "gpt-3.5-turbo",
                temperature = 0.2,
                messages = new[]
                {
                    new { role = "system", content = systemMessage },
                    new { role = "user", content = content },
                }
            };
        }

        private List<string> ParseUserStories(string assistantResponse)
        {
            var userStories = new List<string>();
            var userStoryArray = assistantResponse.Split("\n\nAs a user, ");

            foreach (var userStoryContent in userStoryArray)
            {
                userStories.Add("As a user, " + userStoryContent.Trim());
            }

            return userStories;
        }
    }
}
