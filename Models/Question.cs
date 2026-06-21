namespace SurveyApp.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public int SurveyId { get; set; }
        public Survey? Survey { get; set; }

        public List<Option> Options { get; set; } = new List<Option>();
    }
}