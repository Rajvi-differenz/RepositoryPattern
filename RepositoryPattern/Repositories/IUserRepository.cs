using RepositoryPattern.Models;

namespace RepositoryPattern.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserWithExpensesAsync(int id);
    }
}
