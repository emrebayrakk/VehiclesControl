using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclesControl.Data.Context;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Interfaces;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Data.Repositories
{
    public class BoatRepo : GenericRepo<Boat, BoatRequest, BoatResponse>, IBoatRepo
    {
        public BoatRepo(VehiclesControlContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
