﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclesControl.Domain.Outs
{
    public class CarResponse
    {
        public long Id { get; set; }
        public string Color { get; set; } = string.Empty;
        public bool HeadlightsOn { get; set; }
        public int Wheels { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
