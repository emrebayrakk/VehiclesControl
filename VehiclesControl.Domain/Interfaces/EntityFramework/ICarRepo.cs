using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Domain.Interfaces.EntityFramework
{
    public interface ICarRepo : IGenericRepo<Car, CarRequest, CarResponse>
    {
    }
}
