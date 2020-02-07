using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.API.Concerns
{
    public class Ride
    {
        public string Id { get; set; }

        public User Publisher { get; set; }

        public string PickUp { get; set; }

        public string Drop { get; set; }

        public DateTime StartDate { get; set; }

        public int NumberOfSeats { get; set; }

        public int AvailableSeats { get; set; }

        public double Price { get; set; }

        public Vehicle Vehicle { get; set; }

        public List<ViaPoint> ViaPoints { get; set; }

        public bool AutoApproveRide { get; set; }

        public Status Status { get;set; }
    }
}
