using Microsoft.EntityFrameworkCore;
using ong_project.Domain;
using ong_project.Domain.Repositories;

namespace ong_project.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly OngDbContext _context;

    public CourseRepository(OngDbContext context)
    {
        _context = context;
    }
    
    public async Task CreateClassAsync(Course course)
    {
        await _context.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Course>> GetAllCourses()
    {
        return await _context.Courses.Where(x => x.IsActive).ToListAsync();
    }

    public async Task AddUserToCourseAsync(int courseId, int userId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course != null)
        {
            course.UserId = userId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Course?> GetCourseByIdAsync(string courseId)
    {
        return await _context.Courses.Where(x => x.Token == courseId).FirstOrDefaultAsync();
    }
}