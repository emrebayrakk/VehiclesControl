using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VehiclesControl.Domain.Interfaces.EntityFramework;

namespace VehiclesControl.Data.Repositories.EntityFramework
{
    public class GenericRepo<TEntity, TEntityInput, TEntityOutput> : IGenericRepo<TEntity, TEntityInput, TEntityOutput> where TEntity : class
    {
        private readonly DbContext dbContext;
        private readonly IMapper _mapper;

        public DbSet<TEntity> entity => dbContext.Set<TEntity>();

        public GenericRepo(DbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }
        public int Add(TEntityInput entity)
        {
            try
            {
                var mappedEntity = _mapper.Map<TEntity>(entity);
                this.entity.Add(mappedEntity);
                return dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public TEntity AddEntity(TEntityInput entity)
        {
            try
            {
                var mappedEntity = _mapper.Map<TEntity>(entity);
                this.entity.Add(mappedEntity);
                dbContext.SaveChanges();
                return mappedEntity;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public int Delete(long id)
        {
            try
            {
                var entity = this.entity.Find(id);
                if (entity is not null)
                {
                    this.entity.Remove(entity);
                    return dbContext.SaveChanges();
                }
                return -1;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public TEntityOutput FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                var query = entity.AsQueryable();

                if (predicate != null)
                    query = query.Where(predicate);

                query = ApplyIncludes(query, includes);

                if (noTracking)
                    query = query.AsNoTracking();

                var entities = query.FirstOrDefaultAsync();
                var mapperEntity = _mapper.Map<TEntityOutput>(entities.Result);
                return mapperEntity;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                var query = entity.AsQueryable();

                if (predicate != null)
                    query = query.Where(predicate);

                query = ApplyIncludes(query, includes);

                if (noTracking)
                    query = query.AsNoTracking();

                var entities = query.FirstOrDefault();
                return entities;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<TEntityOutput> GetAll(bool noTracking = true)
        {
            try
            {
                if (noTracking)
                {
                    var mapped = entity.AsNoTracking().ToList();
                    var entities = _mapper.Map<List<TEntityOutput>>(mapped);
                    return entities;
                }
                var result = entity.ToList();
                var mappedResult = _mapper.Map<List<TEntityOutput>>(result);

                return mappedResult;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public int Update(TEntity entity)
        {
            try
            {
                this.entity.Attach(entity);
                dbContext.Entry(entity).State = EntityState.Modified;

                return dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                if (includes != null)
                {
                    foreach (var includeItem in includes)
                    {
                        query = query.Include(includeItem);
                    }
                }

                return query;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
