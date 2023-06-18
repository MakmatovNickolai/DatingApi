using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DataAccess.Repositories;

public interface ICompositeKeyRepository<TEntity> where TEntity : class
{
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task DeleteAsync(params object[] keys);
    Task DeleteAsync(TEntity? entity);
    Task FailIfAnyAsync(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[]? navigationPropertyPaths);
    Task<TEntity?> GetAsync(params object[] keys);
    Task<TEntity> InsertAsync(TEntity entity);
    Task SaveChangesAsync();
    Task<TEntity> UpdateAsync(Action<EntityEntry<TEntity>, DbContext> updateAction, params object[] keys);
}