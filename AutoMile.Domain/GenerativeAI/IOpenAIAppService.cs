using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMile.Domain.GenerativeAI
{
    public interface IOpenAIAppService
    {
        Task<List<string>> CreateUserStoriesFromGPT(string content);
        Task<string> GenerateAcceptanceCriteriaFromUserStory(string userStory);
    }
}
