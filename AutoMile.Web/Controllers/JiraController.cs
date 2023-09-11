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

        [HttpGet("CreateJiraIssues")]
        public async Task<CreateJiraIssueResponseDto> CreateJiraIssues(string projectIdOrKey)
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

        [HttpPost("GetResponseFromGPT")]
        public async Task<List<string>> GetResponseFromGPT(FixVersionDiscDto fixVersionDiscDto)
        {
            var userStories = await _openAIAppService.GetUserStoriesFromGPT(fixVersionDiscDto.Discription);
            return userStories;
        }

        [HttpGet("GetProjectVersionDisc")]
        public async Task<string> GetProjectVersionDisc(string projectIdOrKey)
        {
            var projectVersionDisc = await _jiraService.GetProjectVersionDisc(projectIdOrKey);
            return projectVersionDisc;
        }

        [HttpPost("GetJiraIssues")]
        public async Task<List<GetIssueDto>> GetJiraIssues(string projectKey)
        {
            try
            {
                var result = await _jiraService.GetJiraUserStoriesAsync(projectKey);

                var listOfIssue = result.Select(item => new GetIssueDto
                {
                    Summary = item.Summary,
                    Description = item.Description
                }).ToList();

                return listOfIssue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        [HttpPost("CreateJiraIssue")]
        public async Task<string> CreateJiraIssue(string projectKey, string summary)
        {
            try
            {
                var result = await _jiraService.CreateJiraIssueAsync(projectKey, summary);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
