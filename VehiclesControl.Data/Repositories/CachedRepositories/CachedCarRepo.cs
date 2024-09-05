using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq.Expressions;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Interfaces.EntityFramework;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Data.Repositories.CachedRepositories
{
    public class CachedCarRepo : ICarRepo
    {
        private readonly ICarRepo _carRepo;
        private readonly IMemoryCache _memoryCache;

        public CachedCarRepo(IMemoryCache memoryCache, ICarRepo carRepo)
        {
            _memoryCache = memoryCache;
            _carRepo = carRepo;
        }
        public int Add(CarRequest entity)
        {
            return _carRepo.Add(entity);
        }

        public int Delete(long id)
        {
            return _carRepo.Delete(id);
        }

        public Car FirstOrDefault(Expression<Func<Car, bool>> predicate, bool noTracking = true, params Expression<Func<Car, object>>[] includes)
        {
            return _carRepo.FirstOrDefault(predicate, noTracking, includes);
        }

        public CarResponse FirstOrDefaultAsync(Expression<Func<Car, bool>> predicate, bool noTracking = true, params Expression<Func<Car, object>>[] includes)
        {
            return _carRepo.FirstOrDefaultAsync(predicate, noTracking, includes);
        }

        public List<CarResponse> GetAll(bool noTracking = true)
        {
            List<CarResponse> cr = new List<CarResponse>();
            string key = $"members-getall";
            var response = _memoryCache.GetOrCreate(
                key,
                entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                    return _carRepo.GetAll();
                });
            if (response!=null)
            {
                return response;
            }
            return cr;
        }

        public int Update(Car entity)
        {
            return _carRepo.Update(entity);
        }
    }
}
