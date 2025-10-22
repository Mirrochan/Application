using Application.Abstractions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EventManagmentSystemDbContext _context;

        public UserRepository(EventManagmentSystemDbContext context)
        {
            _context = context;
        }

        public async Task CreateNewUser(UserModel model)
        {
            await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<UserModel?> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<UserModel?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u=>u.Email.ToLower() == email.ToLower());
        }
    }
}