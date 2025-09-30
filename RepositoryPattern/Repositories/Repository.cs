using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Models;
using RepositoryPattern.Models.Data;
using RepositoryPattern.Repositories;
using System.Linq.Expressions;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(predicate).ToListAsync();
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public void Remove(T entity) => _dbSet.Remove(entity);

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }


}
