using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclesControl.Domain.Input
{
    public class CarRequest
    {
        public string Color { get; set; } = string.Empty;
        public bool HeadlightsOn { get; set; }
        public int Wheels { get; set; }
    }
}
