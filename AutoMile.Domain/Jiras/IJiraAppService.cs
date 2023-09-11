using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMile.Domain.Jiras
{
    public interface IJiraAppService
    {
        Task<List<string>> CreateJiraIssuesAsync(string projectKey, string fixVersionName, List<string> userStories);
        Task<string> CreateJiraIssueAsync(string projectKey, string userStoryContent);
        Task<string> GetProjectVersionDisc(string projectIdOrKey, string fixVersionName);
        Task<List<Issue>> GetJiraUserStoriesAsync(string projectKey);
    }
}
