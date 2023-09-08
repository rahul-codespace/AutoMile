using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace AutoMile.Service.Jiras
{
    public class JiraAppService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;


        public JiraAppService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<List<string>> CreateJiraIssuesAsync(List<string> userStories)
        {
            try
            {
                var httpClient = CreateJiraHttpClient();
                var issueCreationTasks = userStories.Select(async userStoryContent =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "/rest/api/2/issue");// Create a new request for each user story

                    var issueRequest = CreateIssueRequestObject(userStoryContent);

                    var jsonContent = JsonConvert.SerializeObject(issueRequest);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                    else
                    {
                        return string.Empty;
                    }
                });

                var createdIssues = await Task.WhenAll(issueCreationTasks);
                return createdIssues.ToList();
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully, log them, or return an error message
                throw ex;
            }
        }

        public async Task<string> GetProjectVersionDisc(string projectIdOrKey)
        {
            try
            {
                var httpClient = CreateJiraHttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"/rest/api/3/project/{projectIdOrKey}/version");

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    dynamic versionResponse = JsonConvert.DeserializeObject(jsonContent);

                    // Extract and return the description field
                    var description = versionResponse.values[0]?.description;

                    if (description != null)
                    {
                        return description.ToString();
                    }
                }

                // Handle non-successful responses here
                // You can log the response or throw a custom exception
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully, log them, or return an error message
                throw ex;
            }
        }



        private HttpClient CreateJiraHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var jiraApiUrl = _configuration["JiraConfig:JiraApiUrl"];
            var jiraAuthorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(_configuration["JiraConfig:JiraAuthorization"]));

            httpClient.BaseAddress = new Uri(jiraApiUrl);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {jiraAuthorization}");

            return httpClient;
        }

        private object CreateIssueRequestObject(string userStoryContent)
        {
            return new
            {
                fields = new
                {
                    project = new { key = "EX" },
                    summary = userStoryContent,
                    description = "Creating an issue using project keys and issue type names using the REST API",
                    issuetype = new { name = "Story" },
                    fixVersions = new[] { new { name = "1.0" } }
                }
            };
        }
    }
}
