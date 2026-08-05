using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Infrastructure.Context;
using Sales.Infrastructure.Interfaces.Repositories;

namespace Sales.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SalesCrudAppDbContext _context;

        public UserRepository(SalesCrudAppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
