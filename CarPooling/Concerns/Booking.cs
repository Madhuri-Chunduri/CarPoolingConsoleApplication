using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Concerns
{
    public class Booking
    {
        public string Id { get; set; }

        public string RideId { get; set; }

        public string PickUp { get; set; }

        public string Drop { get; set; }

        public string BookedBy { get; set; }

        public double Price { get; set; }

        public BookingStatus Status { get; set; }

        public int NumberOfSeatsBooked { get; set; }
    }
}
