using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AutoMile.Service.GenerativeAI
{
    public class OpenAIAppService
    {
        private const string OpenAIEndpoint = "https://api.openai.com/v1/chat/completions";
        private string OpenAIApiKey;
        private readonly IConfiguration _configuration;
        public OpenAIAppService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<string>> GetUserStoriesFromGPT(string content)
        {
            using (HttpClient client = new HttpClient())
            {
                OpenAIApiKey = _configuration["ChatGPT:Key"]!;
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAIApiKey}");

                var requestContent = new
                {
                    model = "gpt-3.5-turbo",
                    temperature = 0.2,
                    messages = new[]
                    {
                        new { role = "system", content = "You are a JIRA AI chatbot. Generate a user story for a software project based on the following information, user story contains the two data summary and discription:" },
                        new { role = "user", content },
                    }
                };

                var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(OpenAIEndpoint, jsonContent);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(responseBody);
                var userStories = new List<string>();

                var assistantResponse = jsonObject["choices"][0]["message"]["content"].ToString();
                var userStoryArray = assistantResponse.Split("\n\nAs a user, ");

                foreach (var userStoryContent in userStoryArray)
                {
                    userStories.Add("As a user, " + userStoryContent.Trim());
                }

                return userStories;
            }
        }
    }
}
