using Atlassian.Jira;
using Microsoft.Extensions.Configuration;

namespace AutoMile.Service.Jiras
{
    public class JiraAppService
    {
        private readonly IConfiguration _configuration;

        public JiraAppService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<string>> CreateJiraIssuesAsync(string projectKey, List<string> userStories)
        {
            var jira = CreateJiraClient();
            var issueKeys = new List<string>();

            foreach (var userStoryContent in userStories)
            {
                try
                {
                    var newIssue = new Issue(jira, projectKey)
                    {
                        Type = "Story",
                        Summary = userStoryContent
                    };

                    await newIssue.SaveChangesAsync(); // Save the new issue

                    issueKeys.Add(newIssue.Key.ToString()); // Store the created issue key
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating issue: {ex.Message}");
                }
            }

            return issueKeys;
        }
        public async Task<string> CreateJiraIssueAsync(string projectKey, string userStoryContent)
        {
            try
            {
                var jira = CreateJiraClient();

                var newIssue = new Issue(jira, projectKey)
                {
                    Type = "Story",
                    Summary = userStoryContent,
                };

                await newIssue.SaveChangesAsync(); // Save the new issue

                return newIssue.Key.ToString(); // Return the created issue key as a string
            }
            catch (Exception ex)
            {
                // Handle exceptions for issue creation here
                Console.WriteLine($"Error creating issue: {ex.Message}");
                throw; // You may want to decide how to handle or propagate exceptions
            }
        }

        public async Task<string> GetProjectVersionDisc(string projectIdOrKey)
        {
            try
            {
                var jira = CreateJiraClient();

                var project = await jira.Projects.GetProjectAsync(projectIdOrKey);
                if (project != null)
                {
                    var versions = await project.GetVersionsAsync();
                    var version = versions.FirstOrDefault();
                    if (version != null)
                    {
                        var versionDescription = version.Description;
                        return versionDescription;
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching project version description: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Issue>> GetJiraUserStoriesAsync(string projectKey)
        {
            try
            {
                var jira = CreateJiraClient();

                var jql = $"project = {projectKey} AND issuetype = Story";
                var issues = await jira.Issues.GetIssuesFromJqlAsync(jql, 100);

                return issues.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
                throw;
            }
        }

        // Function to create the Jira client instance
        private Jira CreateJiraClient()
        {
            return Jira.CreateRestClient(_configuration["JiraConfig:JiraApiUrl"], _configuration["JiraConfig:JiraUserName"], _configuration["JiraConfig:JiraApiToken"]);
        }
    }
}
