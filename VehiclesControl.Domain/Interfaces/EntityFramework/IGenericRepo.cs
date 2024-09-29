using System.Linq.Expressions;

namespace VehiclesControl.Domain.Interfaces.EntityFramework
{
    public interface IGenericRepo<TEntity, TEntityInput, TEntityOutput> where TEntity : class
    {
        int Add(TEntityInput entity);
        TEntity AddEntity(TEntityInput entity);
        int Update(TEntity entity);
        TEntity UpdateEntity(TEntityInput entity);
        int Delete(long id);
        TEntityOutput FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);
        List<TEntityOutput> GetAll(bool noTracking = true);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes);
    }
}
