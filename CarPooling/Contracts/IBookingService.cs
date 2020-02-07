using CarPooling.Concerns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Contracts
{
    public interface IBookingService
    {
        int GetBookingsCount(string rideId, string from, string to);

        void AddBooking(Booking booking);

        void UpdateBooking(Booking booking);
        
        List<Booking> GetUserBookings();

        List<Booking> GetBookingsByRideId(string rideId);

        void CancelAllBookingsByRideId(string rideId);

        double GetTotalIncomeOfRide(Ride ride);

        int AvailableSeats(Ride ride,string from,string to);
    }
}
