using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Concerns
{
    public class VehicleType
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public double MaximumFare { get; set; }

        public int MaximumSeats { get; set; }
    }
}
