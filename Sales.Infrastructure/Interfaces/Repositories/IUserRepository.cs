using Sales.Domain.Entities;

namespace Sales.Infrastructure.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}
