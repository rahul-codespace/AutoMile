namespace AutoMile.Web.Model
{
    public class CreateJiraIssueResponseDto
    {
        public string Discription { get; set; }
        public List<string> UserStories { get; set; }
        public List<string> createdIssues { get; set; }
    }
}
