using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclesControl.Domain.Entities
{
    public class Car : Vehicle
    {
        public bool HeadlightsOn { get; set; }
        public int Wheels { get; set; }
    }
}
