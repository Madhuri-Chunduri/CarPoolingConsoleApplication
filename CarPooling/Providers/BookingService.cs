using CarPooling.Contracts;
using CarPooling.Concerns;
using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CarPooling.Providers
{
    public class BookingService : IBookingService
    {
        public static List<Booking> Bookings = new List<Booking>();

        public int GetBookingsCount(string rideId, string from, string to)
        {
            IRideService rideService = new RideService();
            Ride ride = rideService.GetRideById(rideId);
            List<Booking> approvedBookings = new List<Booking>();
            if (ride.PickUp == from && ride.Drop == to)
            {
                //List<Booking> fromMatchedBookings = Bookings.FindAll(obj => obj.RideId == rideId && obj.Status == 0 && obj.From == from && obj.To!=to);
                //List<Booking> toMatchedBookings = Bookings.FindAll(obj => obj.RideId == rideId && obj.Status == 0 && obj.From != from && obj.To == to);
                approvedBookings = Bookings.FindAll(obj => obj.RideId == rideId && obj.Status == 0 && obj.PickUp == from && obj.Drop == to);
            }
            else
                approvedBookings = Bookings.FindAll(obj => obj.RideId == rideId && obj.Status == 0 && obj.PickUp == from && obj.Drop == to);
            int count = 0;
            foreach (Booking booking in approvedBookings)
            {
                count += booking.NumberOfSeatsBooked;
            }
            return count;
        }

        public void AddBooking(Booking booking)
        {
            Bookings.Add(booking);
        }

        public void UpdateBooking(Booking booking)
        {
            int index = Bookings.IndexOf(booking);
            Bookings[index] = booking;
        }

        public List<Booking> GetBookingsByRideId(string rideId)
        {
            return Bookings.FindAll(obj => obj.RideId == rideId);
        }

        public void CancelAllBookingsByRideId(string rideId)
        {
            Bookings.FindAll(obj => obj.RideId == rideId).ForEach(booking => booking.Status = (BookingStatus)2);
        }

        public int AvailableSeats(Ride ride,string from,string to)
        {
            List<int> Seats = new List<int>();
            List<string> Points = new List<string>();
            Points.Add(ride.PickUp);
            Seats.Add(0);
            List<string> ViaPoints = ride.ViaPoints;
            foreach(string point in ViaPoints)
            {
                Points.Add(point);
                Seats.Add(0);
            }
            Points.Add(ride.Drop);


            int fromIndex = Points.IndexOf(from);
            int toIndex = Points.IndexOf(to);

            int numberOfBookings = 0;

            for (int i = 0; i < toIndex ; i++)
            {
                for (int j = i + 1; j < Points.Count; j++)
                {
                    numberOfBookings = GetBookingsCount(ride.Id, Points[i], Points[j]);
                    for (int k = i; k < j; k++)
                    {
                        Seats[k] += numberOfBookings;
                    }
                }
            }

            int max = 0;
            for(int i=fromIndex;i<toIndex;i++)
            {
                if (Seats[i] > max) max = Seats[i];
            }
            return ride.NumberOfSeats - max;
        }

        public List<Booking> GetUserBookings()
        {
            return Bookings.FindAll(obj => obj.BookedBy == LoginActions.currentUser.Id);
        }

        public double GetTotalIncomeOfRide(Ride ride)
        {
            double totalIncome = 0;
            List<Booking> bookings = Bookings.FindAll(obj => obj.RideId == ride.Id);
            foreach (Booking booking in bookings)
            {
                totalIncome += booking.Price;
            }
            return totalIncome;
        }
    }
}
     