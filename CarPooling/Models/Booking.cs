using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Models
{
    public class Booking
    {
        public string Id { get; set; }

        public string RideId { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string BookedBy { get; set; }

        public double Price { get; set; }

        public BookingStatus Status { get; set; }

        public int NumberOfSeatsBooked { get; set; }
    }
}
