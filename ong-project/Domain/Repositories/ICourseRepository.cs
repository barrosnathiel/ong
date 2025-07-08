namespace ong_project.Domain.Repositories;

public interface ICourseRepository
{
    Task CreateClassAsync(Course course);
    Task<List<Course>> GetAllCourses();
    Task AddUserToCourseAsync(int courseId, int userId);
    Task<Course?> GetCourseByIdAsync(string courseId);
}