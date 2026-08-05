using Sales.Application.Dtos;

namespace Sales.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> AuthenticateAsync(string username, string password);
    }
}
