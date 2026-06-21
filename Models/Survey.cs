namespace SurveyApp.Models;

public class Survey
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty; 
    public string Description { get; set; } = string.Empty;
    
    public string CreatorId { get; set; } = string.Empty; 

    public List<Option> Options { get; set; } = new List<Option>();
}