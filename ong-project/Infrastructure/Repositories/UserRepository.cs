using Microsoft.EntityFrameworkCore;
using ong_project.Common;
using ong_project.Domain;
using ong_project.Domain.Repositories;

namespace ong_project.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly OngDbContext _context;

    public UserRepository(OngDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByTokenAsync(string token)
    {
        return await _context.Users.Where(x => x.Token == token).FirstOrDefaultAsync();
    }

    public async Task CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.Where(x => x.UserType != Constants.ADMIN_ROLE).ToListAsync();
    }
}