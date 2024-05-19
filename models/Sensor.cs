using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanSat1.models
{
    public class Sensor
    {
        public string? Temperature { get; set; }
        public string? Humidity { get; set; }
        public string? Gas { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
