namespace ong_project.Application.Requests;

public class CourseRequest
{
    public string Name { get; set; }
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
    public string TotalHours { get; set; }
}