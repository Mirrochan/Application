using Domain.Models;

namespace Application.Abstractions
{
    public interface IUserRepository
    {
        Task<UserModel?> GetByIdAsync(Guid id);
        Task<UserModel?> GetByEmailAsync(string email);
        Task<bool> UserExistsAsync(string email);
        Task CreateNewUser(UserModel model);
        
    }
}