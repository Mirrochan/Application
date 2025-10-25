using Application.Abstractions;
using Application.DTOs;
using Application.Validators;
using Domain.Models;
using FluentValidation;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
           
            var validationResult = await _registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (await _userRepository.UserExistsAsync(request.Email))
            {
                throw new Exception("Account already exists");
            }

            var userModel = new UserModel
            {
                Id = Guid.NewGuid(),
                Email = request.Email.ToLower().Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PasswordHash = _passwordHasher.Generate(request.Password)
            };

            await _userRepository.CreateNewUser(userModel);
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
        
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var user = await _userRepository.GetByEmailAsync(request.Email.ToLower().Trim());
            if (user == null)
            {
                throw new Exception("Incorrect email");
            }

            var result = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!result)
            {
                throw new Exception("Incorrect password");
            }

            var token = _jwtProvider.GenerateToken(user);


            return token;
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}