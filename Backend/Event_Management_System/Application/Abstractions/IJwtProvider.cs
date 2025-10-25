using Domain.Models;

namespace Application.Abstractions
{
    public interface IJwtProvider
    {
        string GenerateToken(UserModel user);
    }
}