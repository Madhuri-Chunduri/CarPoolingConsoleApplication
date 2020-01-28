using CarPooling.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Interfaces
{
    public interface IBookingService
    {
        int GetBookingsCount(string rideId, string from, string to);

        void AddBooking(Booking booking);

        void ApproveBooking(string bookingId);

        void UpdateBooking(Booking booking);

        void RejectBooking(string bookingId);

        List<Booking> GetUserBookings();

        Ride GetRideById(string bookingId);

        List<Booking> GetBookingsByRideId(string rideId);

        void CancelAllBookingsByRideId(string rideId);

        void CancelBookingById(string bookingId);

        double GetTotalIncomeOfRide(Ride ride);

        int AreSeatsAvailable(Ride ride);

        int AreSeatsAvailableBetweenViaPoints(Ride ride, string viaPointFrom, string viaPointTo);

        int AreSeatsAvailableToViaPoint(Ride ride, string viaPointTo);

        int AreSeatsAvailableFromViaPoint(Ride ride, string viaPointFrom);
    }
}
