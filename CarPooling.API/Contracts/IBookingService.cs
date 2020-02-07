using CarPooling.API.Concerns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.API.Contracts
{
    public interface IBookingService
    {
        int GetBookingsCount(string rideId, string from, string to);

        bool AddBooking(Booking booking);

        bool UpdateBooking(Booking booking);

        List<Booking> GetBookingsByUserId(string userId);

        List<Booking> GetBookingsByRideId(string rideId);

        bool CancelAllBookingsByRideId(string rideId);

        double GetTotalIncomeOfRide(Ride ride);

        int AvailableSeats(Ride ride,string from,string to);
    }
}
