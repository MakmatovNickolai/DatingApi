using Common.Exceptions;
using DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DataAccess.Repositories;

public interface IRepository<TEntity, TKey> where TEntity : Entity<TKey>
{
    DbSet<TEntity> Set();
    DbSet<TDirectEntity> Set<TDirectEntity>() where TDirectEntity : class;
    Task<TEntity> GetAsync(TKey key, params Expression<Func<TEntity, object>>[]? navigationPropertyPaths);
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> searchConditions, params Expression<Func<TEntity, object>>[]? navigationPropertyPaths);
    IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[]? navigationPropertyPaths);
    DbSet<TEntity> GetDbSet();
    Task<TEntity> InsertAsync(TEntity entity);
    Task<IEnumerable<TEntity>> InsertRangeAsync(IEnumerable<TEntity> entities);
    Task<TEntity> UpdateAsync(TKey id, Action<EntityEntry<TEntity>, DbContext> updateAction);
    Task DeleteAsync(TEntity entity);
    Task DeleteAsync(Expression<Func<TEntity, bool>> searchConditions);
    Task DeleteAsync(TKey key);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task FailIfAnyAsync(Expression<Func<TEntity, bool>> predicate);
    Task SaveChanges();
}

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : Entity<TKey>
{
    private readonly ApplicationDbContext _context;
    public Repository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }

    public DbSet<TEntity> Set()
    {
        return _context.Set<TEntity>();
    }

    public DbSet<TDirectEntity> Set<TDirectEntity>() where TDirectEntity : class
    {
        return _context.Set<TDirectEntity>();
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

    public async Task DeleteAsync(TKey key)
    {
        var entity = await GetTrackingAsync(key);
        await DeleteAsync(entity);
    }
    public async Task DeleteAsync(Expression<Func<TEntity, bool>> searchConditions)
    {
        var entity = await GetTrackingAsync(searchConditions);
        await DeleteAsync(entity);
    }

    public Task<TEntity> GetAsync(TKey key, params Expression<Func<TEntity, object>>[]? navigationPropertyPaths)
    {
        var set = GetAll(navigationPropertyPaths);

        return set.FirstOrDefaultAsync(x => x.Id.Equals(key));
    }

    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> searchConditions, params Expression<Func<TEntity, object>>[]? navigationPropertyPaths)
    {
        var set = GetAll(navigationPropertyPaths);

        return set.FirstOrDefaultAsync(searchConditions);
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

    public DbSet<TEntity> GetDbSet()
    {
        return _context.Set<TEntity>();
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<IEnumerable<TEntity>> InsertRangeAsync(IEnumerable<TEntity> entities)
    {
        await _context.AddAsync(entities);
        await _context.SaveChangesAsync();

        return entities;
    }

    public async Task<TEntity> UpdateAsync(TKey id, Action<EntityEntry<TEntity>, DbContext> updateAction)
    {
        var entity = await GetTrackingAsync(id);
        updateAction(_context.Entry(entity), _context);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task FailIfAnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var anyResult = await _context.Set<TEntity>().AsNoTracking().AnyAsync(predicate);
        if (anyResult)
        {
            throw new DuplicatedObjectException();
        }
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

    private async Task<TEntity?> GetTrackingAsync(TKey key, params Expression<Func<TEntity, object>>[]? navigationPropertyPaths)
    {
        var set = _context.Set<TEntity>().AsQueryable();
        var includes = navigationPropertyPaths ?? Enumerable.Empty<Expression<Func<TEntity, object>>>();

        foreach (var includeItem in includes)
        {
            set = set.Include(includeItem);
        }

        var entity = await set.FirstOrDefaultAsync(x => x.Id.Equals(key));
        return entity;
    }

    private async Task<TEntity?> GetTrackingAsync(Expression<Func<TEntity, bool>> searchConditions, params Expression<Func<TEntity, object>>[]? navigationPropertyPaths)
    {
        var set = _context.Set<TEntity>().AsQueryable();
        var includes = navigationPropertyPaths ?? Enumerable.Empty<Expression<Func<TEntity, object>>>();

        foreach (var includeItem in includes)
        {
            set = set.Include(includeItem);
        }

        var entity = await set.FirstOrDefaultAsync(searchConditions);
        return entity;
    }
}