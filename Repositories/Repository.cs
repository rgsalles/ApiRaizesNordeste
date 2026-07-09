using System.Linq.Expressions;
using ApiRaizesNordeste.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiRaizesNordeste.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id) =>
        await DbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await DbSet.ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await DbSet.Where(predicate).ToListAsync();

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
        await DbSet.FirstOrDefaultAsync(predicate);

    public async Task AddAsync(T entity) =>
        await DbSet.AddAsync(entity);

    public void Update(T entity) =>
        DbSet.Update(entity);
}
