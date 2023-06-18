using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DataAccess.Repositories;

public class CompositeKeyRepository<TEntity> : ICompositeKeyRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _context;

    public CompositeKeyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return _context.Set<TEntity>().CountAsync(predicate);
    }

    public async Task DeleteAsync(TEntity? entity)
    {
        if (entity == null)
        {
            return;
        }

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(params object[] keys)
    {
        var entity = _context.Set<TEntity>().FindAsync(keys);
        await DeleteAsync(entity);
    }

    public async Task<TEntity?> GetAsync(params object[] keys)
    {
        var entity = await _context.Set<TEntity>().FindAsync(keys);
        return entity;
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }


    public async Task<TEntity> UpdateAsync(Action<EntityEntry<TEntity>, DbContext> updateAction, params object[] keys)
    {
        var entity = await _context.Set<TEntity>().FindAsync(keys);
        updateAction(_context.Entry(entity), _context);
        await _context.SaveChangesAsync();
        return entity;
    }

    public IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[]? navigationPropertyPaths)
    {
        var set = _context.Set<TEntity>().AsNoTracking().AsQueryable();
        var includes = navigationPropertyPaths ?? Enumerable.Empty<Expression<Func<TEntity, object>>>();

        foreach (var includeItem in includes)
        {
            set = set.Include(includeItem);
        }

        return set;
    }

    public async Task FailIfAnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var anyResult = await _context.Set<TEntity>().AsNoTracking().AnyAsync(predicate);
        if (anyResult)
        {
            throw new DuplicatedObjectException();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
