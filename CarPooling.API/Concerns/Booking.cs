using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.API.Concerns
{
    public class Booking
    {
        public string Id { get; set; }

        public Ride Ride { get; set; }

        public string PickUp { get; set; }

        public string Drop { get; set; }

        public User BookedBy { get; set; }

        public DateTime BookingTime { get; set; }

        public double Price { get; set; }

        public Status Status { get; set; }

        public int NumberOfSeatsBooked { get; set; }
    }
}
