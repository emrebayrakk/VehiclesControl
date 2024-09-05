using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclesControl.Domain.Entities;

namespace VehiclesControl.Domain.Interfaces.EntityFramework
{
    public interface IJwtProvider
    {
        string CreateToken(User user);
    }
}
