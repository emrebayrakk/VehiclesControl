using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclesControl.Domain.Entities
{
    [Table("Cars")]
    public class Car : Vehicle
    {
        [Column("HeadlightsOn")]
        public bool HeadlightsOn { get; set; }
        [Column("Wheels")]
        public int Wheels { get; set; }
    }
}
