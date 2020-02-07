using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Concerns
{
    public class Ride
    {
        public string Id { get; set; }

        public string PublisherId { get; set; }

        public string PickUp { get; set; }

        public string Drop { get; set; }

        public DateTime Date { get; set; }

        public List<string> ViaPoints { get; set; }

        public int NumberOfSeats { get; set; }

        public int AvailableSeats { get; set; }

        public double Price { get; set; }

        public Vehicle Vehicle { get; set; }

        public bool AutoApproveRide { get; set; }

        public RideStatus Status { get;set; }

        public VehicleType VehicleType { get; set; }
    }
}
