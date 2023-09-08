using AutoMile.Service.GenerativeAI;
using AutoMile.Service.Jiras;
using AutoMile.Web.Model;
using Microsoft.AspNetCore.Mvc;

namespace AutoMile.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JiraController : ControllerBase
    {
        private readonly JiraAppService _jiraService;
        private readonly OpenAIAppService _openAIAppService;
        
        public JiraController(JiraAppService jiraService, OpenAIAppService openAIAppService)
        {
            _jiraService = jiraService;
            _openAIAppService = openAIAppService;
        }

        [HttpGet("CreateJiraIssue")]
        public async Task<CreateJiraIssueResponseDto> CreateJiraIssue(string projectIdOrKey)
        {
            var projectVersionDisc = await _jiraService.GetProjectVersionDisc(projectIdOrKey);
            var userStories = await _openAIAppService.GetUserStoriesFromGPT(projectVersionDisc);
            var createdIssues = await _jiraService.CreateJiraIssuesAsync(userStories);
            var returnDTO = new CreateJiraIssueResponseDto
            {
                Discription = projectVersionDisc,
                UserStories = userStories,
                createdIssues = createdIssues
            };
            return returnDTO;
        }

        [HttpGet("GetResponseFromGPT")]
        public async Task<List<string>> GetResponseFromGPT([FromBody] string content)
        {
            var userStories = await _openAIAppService.GetUserStoriesFromGPT(content);
            return userStories;
        }

        [HttpGet("GetProjectVersionDisc")]
        public async Task<string> GetProjectVersionDisc(string projectIdOrKey)
        {
            var projectVersionDisc = await _jiraService.GetProjectVersionDisc(projectIdOrKey);
            return projectVersionDisc;
        }
    }
}
