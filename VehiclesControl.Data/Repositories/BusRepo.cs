using AutoMapper;
using VehiclesControl.Data.Context;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Interfaces;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Data.Repositories
{
    public class BusRepo : GenericRepo<Bus, BusRequest,BusResponse>, IBusRepo
    {
        public BusRepo(VehiclesControlContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
