using CarPooling.API.Contracts;
using CarPooling.API.Concerns;
using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace CarPooling.API.Providers
{
    public class BookingService : IBookingService
    {
        public IConfiguration configuration;

        public string connectionString;

        public BookingService(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetSection("ConnectionString").Value;
        }
       
        public int GetBookingsCount(string rideId,string from,string to)
        {
            string query = "select NumberOfSeatsBooked from Booking where StatusId in (select Id from Status where Type='Booking' and Value='Approved') and RideId='" + rideId + 
                "' and PickUp='" + from + "' and [Drop]='" + to + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            List<Booking> approvedBookings = extensionObject.GetAllItems<Booking>();

            int count = 0;
            if (approvedBookings == null) return count;
            foreach (Booking booking in approvedBookings)
            {
               count += booking.NumberOfSeatsBooked;
            }
            return count;
        }

        public bool AddBooking(Booking booking)
        {
            string query = "insert into Booking(Id,RideId,PickUp,[Drop],BookedBy,Price,Status,BookingTime,NumberOfSeatsBooked)" +
                " values(@Id,@RideId,@PickUp,@Drop,@BookedBy,@Price,@Status,@BookingTime,@NumberOfSeats)";
            booking.Id = Guid.NewGuid().ToString();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", booking.Id);
            parameters.Add("RideId", booking.Ride.Id);
            parameters.Add("PickUp", booking.PickUp);
            parameters.Add("Drop", booking.Drop);
            parameters.Add("BookedBy", booking.BookedBy);
            parameters.Add("Price", booking.Price);
            parameters.Add("Status", booking.Status.Id);
            parameters.Add("BookingTime", booking.BookingTime);
            parameters.Add("NumberOfSeatsBooked", booking.NumberOfSeatsBooked);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<Booking>(parameters);
        }

        public bool UpdateBooking(Booking booking)
        {
            string query = "update Booking set PickUp=@PickUp,[Drop]=@Drop" +
                ",Status=@Status,NumberOfSeatsBooked=@NumberOfSeatsBooked where Id=@Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", booking.Id);
            parameters.Add("PickUp", booking.PickUp);
            parameters.Add("Drop", booking.Drop);
            parameters.Add("Status", booking.Status.Value);
            parameters.Add("NumberOfSeatsBooked", booking.NumberOfSeatsBooked);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            //new { booking.Id, booking.PickUp, booking.Drop, booking.Status, booking.NumberOfSeatsBooked });
            return extensionObject.AddOrUpdateItem<Booking>(parameters);
        }

        public List<Booking> GetBookingsByRideId(string rideId)
        {
            //string query = "select * from Booking where RideId='" + rideId + "'";
            //return query.GetAllItems<Booking>();

            string query = "select b.PickUp,b.[Drop],b.Price,b.NumberOfSeatsBooked,s.Value,customer.Name,customer.PhoneNumber,r.PickUp,r.[Drop] " +
                "from Booking b inner join [User] customer on b.BookedBy = customer.Id inner join Status s on s.Id=b.StatusId inner join Ride r on b.RideId = r.Id where b.RideId='"+rideId+"'";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    return connection.Query<Booking,Status, User, Ride, Booking>(query, map: (booking, status, customer, ride) =>
                    {
                        booking.BookedBy = customer;
                        booking.Ride = ride;
                        booking.Status = status;
                        return booking;
                    },
                   splitOn: "Value,Name,PickUp"
                   ).ToList();
                }
            }
            catch(Exception exception)
            {
                ILogService logService = new LogService(connectionString);
                logService.LogException(exception.GetType().ToString(), "GetBookingsByRideId");
                return null;
            }
        }

        public bool CancelAllBookingsByRideId(string rideId)
        {
            string query = "update Booking set Status='Cancelled' where RideId='" + rideId + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            Booking booking = extensionObject.GetItem<Booking>();
            if (booking == null) return false;
            return true;
        }

        public List<Booking> GetBookingsByUserId(string userId)
        {
            string query = "select b.PickUp,b.[Drop],b.Price,b.NumberOfSeatsBooked,s.Value,u.Name,u.PhoneNumber,r.StartDate,r.PickUp,r.[Drop]" +
                "from Booking b inner join Ride r on b.RideId=r.Id inner join Status s on b.StatusId=s.Id inner join [User] u on u.Id=r.PublisherId where b.BookedBy='"+userId+"'";
            // string query = "select * from Booking where BookedBy='" + userId + "'";
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    return connection.Query<Booking, Status, User, Ride, Booking>(query, map: (booking,status, publisher, ride) =>
                    {
                        booking.Status = status;
                        booking.Ride = ride;
                        ride.Publisher = publisher;
                        return booking;
                    },
                   splitOn: "Value,Name,StartDate"
                   ).ToList();
                }
            }
            catch(Exception exception)
            {
                ILogService logService = new LogService(connectionString);
                logService.LogException(exception.GetType().ToString(), "GetUserBookings");
                return null;
            }
        } 

        public double GetTotalIncomeOfRide(Ride ride)
        {
            double totalIncome = 0;
            List<Booking> bookings = GetBookingsByRideId(ride.Id);
            foreach(Booking booking in bookings)
            {
                totalIncome += booking.Price;
            }
            return totalIncome;
        }

        public int AvailableSeats(Ride ride,string from,string to)
        {
            List<int> Seats = new List<int>();
            List<string> Points = new List<string>();
            Points.Add(ride.PickUp);
            Seats.Add(0);

            IViaPointService viaPointService = new ViaPointService(configuration);
            List<string> ViaPoints = viaPointService.GetAllViaPoints(ride.Id);
            foreach(string point in ViaPoints)
            {
                Points.Add(point);
                Seats.Add(0);
            }
            Points.Add(ride.Drop);

            int fromIndex = Points.IndexOf(from);
            int toIndex = Points.IndexOf(to);
            if (fromIndex == -1 || toIndex == -1) return 0;

            int numberOfBookings = 0;

            for (int i = 0; i < toIndex ; i++)
            {
                for (int j = i + 1; j < Points.Count; j++)
                {
                    numberOfBookings = GetBookingsCount(ride.Id, Points[i], Points[j]);
                    if (numberOfBookings == 0) continue;
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
    }
}
     