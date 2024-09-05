using AutoMapper;
using VehiclesControl.Data.Context;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Interfaces.EntityFramework;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Data.Repositories.EntityFramework
{
    public class BoatRepo : GenericRepo<Boat, BoatRequest, BoatResponse>, IBoatRepo
    {
        public BoatRepo(VehiclesControlContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
