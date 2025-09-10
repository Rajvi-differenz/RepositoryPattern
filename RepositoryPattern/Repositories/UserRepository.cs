using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Models.Data;
using RepositoryPattern.Models;

namespace RepositoryPattern.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetUserWithExpensesAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
