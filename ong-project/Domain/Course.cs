namespace ong_project.Domain;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
    public string TotalHours { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public bool IsActive { get; set; }
    public string Token { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}