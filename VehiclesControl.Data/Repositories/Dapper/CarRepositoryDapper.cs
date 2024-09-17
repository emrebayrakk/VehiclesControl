using Microsoft.Extensions.Logging;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Interfaces.Dapper;

namespace VehiclesControl.Data.Repositories.Dapper
{
    public class CarRepositoryDapper : GenericRepository<Domain.Entities.Car>, ICarRepositoryDapper
    {
        public CarRepositoryDapper(ISqlToolsProvider sqlToolsProvider, IDapperContext dapperContext, ILogger<GenericRepository<Car>> logger) : base(sqlToolsProvider, dapperContext, logger)
        {
        }
    }
}
