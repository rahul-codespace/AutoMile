using AutoMile.Domain.GenerativeAI;
using AutoMile.Domain.Jiras;
using AutoMile.Service.Jiras;
using AutoMile.Web.Model;
using Microsoft.AspNetCore.Mvc;

namespace AutoMile.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JiraController : ControllerBase
    {
        private readonly IJiraAppService _jiraService;
        private readonly IOpenAIAppService _openAIAppService;
        

        public JiraController(JiraAppService jiraService, IOpenAIAppService openAIAppService)
        {
            _jiraService = jiraService;
            _openAIAppService = openAIAppService;
        }

        [HttpGet("CreateJiraIssues")]
        public async Task<CreateJiraIssueResponseDto> CreateJiraIssues(string projectIdOrKey, string fixVersionName)
        {
            var projectVersionDisc = await _jiraService.GetProjectVersionDisc(projectIdOrKey, fixVersionName);
            var userStories = await _openAIAppService.CreateUserStoriesFromGPT(projectVersionDisc);
            var createdIssues = await _jiraService.CreateJiraIssuesAsync(projectIdOrKey, fixVersionName,userStories);
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
            var userStories = await _openAIAppService.CreateUserStoriesFromGPT(fixVersionDiscDto.Discription);
            return userStories;
        }

        [HttpGet("GetProjectVersionDisc")]
        public async Task<string> GetProjectVersionDisc(string projectIdOrKey, string fixVersionName)
        {
            var projectVersionDisc = await _jiraService.GetProjectVersionDisc(projectIdOrKey, fixVersionName);
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

        [HttpPost("GenerateAcceptanceCriteriaFromUserStory")]
        public async Task<string> GetUserAcceptanceCriteria(string userStory)
        {
            try
            {
                var result = await _openAIAppService.GenerateAcceptanceCriteriaFromUserStory(userStory);
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
