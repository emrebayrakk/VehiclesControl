using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclesControl.Domain.Entities
{
    public abstract class Vehicle
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("Color")]
        public string Color { get; set; } = string.Empty;

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
       
    }
}
