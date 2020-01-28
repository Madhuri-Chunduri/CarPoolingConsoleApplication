using CarPooling.Interfaces;
using CarPooling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarPooling.Services
{
    public class BookingService : IBookingService
    {
        public static List<Booking> Bookings = new List<Booking>();

        public int GetBookingsCount(string rideId,string from,string to)
        {
            IRideService rideService = new RideService();
            Ride ride = rideService.GetRide(rideId);
            List<Booking> approvedBookings = new List<Booking>();
            if (ride.From==from && ride.To==to)
            {
                //List<Booking> fromMatchedBookings = Bookings.FindAll(obj => obj.RideId == rideId && obj.Status == 0 && obj.From == from && obj.To!=to);
                //List<Booking> toMatchedBookings = Bookings.FindAll(obj => obj.RideId == rideId && obj.Status == 0 && obj.From != from && obj.To == to);
                approvedBookings = Bookings.FindAll(obj => obj.RideId == rideId && obj.Status == 0 && obj.From == from && obj.To == to);
            }
            else
                approvedBookings =  Bookings.FindAll(obj => obj.RideId == rideId && obj.Status == 0 && obj.From==from && obj.To==to);
            int count = 0;
            foreach(Booking booking in approvedBookings)
            {
                count += booking.NumberOfSeatsBooked;
            }
            return count;
        }

        public void AddBooking(Booking booking)
        {
            Bookings.Add(booking);
        }

       //public Booking GetBookingByRideId(string rideId)
        //{
        //    return Bookings.Find(obj => obj.RideId == rideId && obj.BookedBy == LoginActions.currentUser.Id);
       // }

       // public List<Ride> GetBookedRidesByUserId(string bookedBy)
       // {
       //     List<Ride> rides = new List<Ride>();
       //     List<Booking> bookedRides = Bookings.FindAll(obj => obj.BookedBy == bookedBy);
       //     IRideService rideService = new RideService();
      //      foreach(Booking booking in bookedRides)
       //     {
        //        rides.Add(rideService.GetRide(booking.RideId));
        //    }
        //    return rides;
       // }

        public void ApproveBooking(string bookingId)
        {
           Bookings.Find(obj => (obj.Id == bookingId)).Status=0;
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
           Bookings.FindAll(obj => obj.RideId == rideId).ForEach(booking => booking.Status=(BookingStatus)2);
        }

        //public void CancelBookingByRideId(string rideId)
        //{
        //    int index = Bookings.IndexOf(Bookings.Find(obj => obj.RideId == rideId && obj.BookedBy == LoginActions.currentUser.Id));
        //    Bookings[index].Status = (BookingStatus)2;
        //}

        public void CancelBookingById(string bookingId)
        {
             Bookings.Find(obj => obj.Id == bookingId).Status = (BookingStatus)2;
        }

        public void RejectBooking(string bookingId)
        {
            Bookings.Find(obj => (obj.Id == bookingId)).Status = (BookingStatus)3;
        }

        public int AreSeatsAvailable(Ride ride)
        {
            int numberOfBookings = GetBookingsCount(ride.Id, ride.From, ride.To);
            for(int i=0;i<ride.ViaPoints.Count;i++)
            {
                numberOfBookings += GetBookingsCount(ride.Id, ride.From, ride.ViaPoints[i]);
            }
            int max = numberOfBookings;
            if(ride.NumberOfSeats-max>0)
            {
                for(int i=0;i<ride.ViaPoints.Count;i++)
                {
                    numberOfBookings = 0;
                    for (int j=i+1;j<ride.ViaPoints.Count;j++)
                    {
                        numberOfBookings += GetBookingsCount(ride.Id, ride.ViaPoints[i], ride.ViaPoints[j]);
                    }
                    numberOfBookings += GetBookingsCount(ride.Id, ride.ViaPoints[i], ride.To);
                    if (max < numberOfBookings) max = numberOfBookings;
                }
                return (ride.NumberOfSeats-max);
            }
            return 0;
        }

        public int AreSeatsAvailableFromViaPoint(Ride ride, string viaPointFrom)
        {
            int index = ride.ViaPoints.IndexOf(viaPointFrom);
            int numberOfBookings = 0;
            for (int i = index + 1; i < ride.ViaPoints.Count; i++)
            {
                numberOfBookings += GetBookingsCount(ride.Id, ride.From, ride.ViaPoints[i]);
            }
            numberOfBookings += GetBookingsCount(ride.Id, ride.From, ride.To);

            if (ride.NumberOfSeats - numberOfBookings >= 0)
            {
                for (int i = 0; i < ride.ViaPoints.Count; i++)
                {
                    numberOfBookings += GetBookingsCount(ride.Id, ride.ViaPoints[i], ride.To);
                    for (int j = index + 1; j < ride.ViaPoints.Count; j++)
                    {
                        numberOfBookings += GetBookingsCount(ride.Id, ride.ViaPoints[i], ride.ViaPoints[j]);
                    }
                }
                int availableSeats = ride.NumberOfSeats - numberOfBookings;
                if (availableSeats >= 0) return availableSeats;
            }
            return 0;
        }

        public int AreSeatsAvailableToViaPoint(Ride ride,string viaPointTo)
        {
            int index = ride.ViaPoints.IndexOf(viaPointTo);
            int numberOfBookings = 0;
            for(int i=0;i<ride.ViaPoints.Count;i++)
            {
                numberOfBookings += GetBookingsCount(ride.Id, ride.From, ride.ViaPoints[i]);
            }
            numberOfBookings += GetBookingsCount(ride.Id, ride.From, ride.To);

            if(ride.NumberOfSeats-numberOfBookings>0)
            {
                for(int i=0;i<index;i++)
                {
                    numberOfBookings += GetBookingsCount(ride.Id, ride.ViaPoints[i], ride.To);
                    for (int j=index;j<ride.ViaPoints.Count;j++)
                    {
                        numberOfBookings += GetBookingsCount(ride.Id, ride.ViaPoints[i], ride.ViaPoints[j]);
                    }
                }
                int availableSeats = ride.NumberOfSeats - numberOfBookings;
                if (availableSeats >= 0) return availableSeats;
            }
            return 0;
        }

        public int AreSeatsAvailableBetweenViaPoints(Ride ride,string viaPointFrom,string viaPointTo)
        {
            int fromIndex = ride.ViaPoints.IndexOf(viaPointFrom);
            int toIndex = ride.ViaPoints.IndexOf(viaPointTo);
            int numberOfBookings = 0;

            for(int i=fromIndex+1;i<ride.ViaPoints.Count;i++)
            {
                numberOfBookings += GetBookingsCount(ride.Id, ride.From, ride.ViaPoints[i]);
            }
            numberOfBookings += GetBookingsCount(ride.Id, ride.From, ride.To);

            if (ride.NumberOfSeats - numberOfBookings>0)
            {
                for (int i = 0; i <= fromIndex; i++)
                {
                    numberOfBookings += GetBookingsCount(ride.Id, ride.ViaPoints[i], ride.To);
                    for (int j = fromIndex + 1; j < ride.ViaPoints.Count; j++)
                    {
                        numberOfBookings += GetBookingsCount(ride.Id, ride.ViaPoints[i], ride.ViaPoints[j]);
                    }
                }
                int availableSeats = ride.NumberOfSeats - numberOfBookings;
                if (availableSeats >= 0) return availableSeats;
            }
            return 0;
        }

        public List<Booking> GetUserBookings()
        {
            return Bookings.FindAll(obj => obj.BookedBy == LoginActions.currentUser.Id);
        }

        public Ride GetRideById(string rideId)
        {
            IRideService rideService = new RideService();
            return rideService.GetRide(rideId);
        }

        public double GetTotalIncomeOfRide(Ride ride)
        {
            double totalIncome = 0;
            List<Booking> bookings = Bookings.FindAll(obj => obj.RideId == ride.Id);
            foreach(Booking booking in bookings)
            {
                totalIncome += booking.Price;
            }
            return totalIncome;
        }
    }
}
