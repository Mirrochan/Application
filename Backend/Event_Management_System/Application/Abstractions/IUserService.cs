

using Application.DTOs;

namespace Application.Abstractions
{
    public interface IUserService
    {
        Task<UserResponse?> GetUserByIdAsync(Guid userId);
        Task<string> LoginAsync(LoginRequest request);
        Task RegisterAsync(RegisterRequest request);
    }
}