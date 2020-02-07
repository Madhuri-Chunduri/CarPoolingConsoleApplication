using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarPooling.API.Concerns
{
    public class VehicleType
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double MaximumFare { get; set; }

        public int MaximumSeats { get; set; }
    }
}
