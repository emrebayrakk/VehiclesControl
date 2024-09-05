using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclesControl.Data.Context;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Interfaces.EntityFramework;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Data.Repositories.EntityFramework
{
    public class UserRepo : GenericRepo<User, UserRequest, UserResponse>, IUserRepo
    {
        public UserRepo(VehiclesControlContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
