using System.Security.Cryptography;
using System.Text;
using Sales.Application.Dtos;
using Sales.Application.Interfaces;
using Sales.Infrastructure.Interfaces.Repositories;

namespace Sales.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResponseDto> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Usuario o contraseña incorrectos."
                };
            }

            var passwordHash = HashPassword(password);

            if (user.PasswordHash != passwordHash)
            {
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Usuario o contraseña incorrectos."
                };
            }

            return new LoginResponseDto
            {
                IsSuccess = true,
                Message = "Autenticación exitosa.",
                Username = user.Username
            };
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
