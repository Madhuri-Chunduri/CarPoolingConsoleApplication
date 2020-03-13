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

        public int AddBooking(Booking booking)
        {
            IRideService rideService = new RideService(configuration);
            Ride ride = rideService.GetRideById(booking.Ride.Id);
            ride.Id = booking.Ride.Id;
            int availableSeats = AvailableSeats(ride, booking.PickUp, booking.Drop);
            if (availableSeats >= booking.NumberOfSeatsBooked)
            {
                string query = "insert into Booking(Id,RideId,PickUp,[Drop],BookedBy,Price,StatusId,BookingTime,NumberOfSeatsBooked)" +
                    " values(@Id,@RideId,@PickUp,@Drop,@BookedBy,@Price,@StatusId,@BookingTime,@NumberOfSeatsBooked)";
                booking.Id = Guid.NewGuid().ToString();
                string statusQuery = "select * from Status where Type='Booking' and Value='Pending'";
                ExtensionObject statusExtensionObject = new ExtensionObject()
                {
                    Query = statusQuery,
                    ConnectionString = connectionString
                };

                string statusId = statusExtensionObject.GetItem<Status>().Id;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", booking.Id);
                parameters.Add("RideId", booking.Ride.Id);
                parameters.Add("PickUp", booking.PickUp);
                parameters.Add("Drop", booking.Drop);
                parameters.Add("BookedBy", booking.BookedBy.Id);
                parameters.Add("Price", booking.Price);
                parameters.Add("StatusId", statusId);
                parameters.Add("BookingTime", booking.BookingTime);
                parameters.Add("NumberOfSeatsBooked", booking.NumberOfSeatsBooked);
                ExtensionObject extensionObject = new ExtensionObject()
                {
                    Query = query,
                    ConnectionString = connectionString
                };
                if (extensionObject.AddOrUpdateItem<Booking>(parameters)) return 1;
                else return 0;
            }
            else return -1;
        }

        public int UpdateBooking(Booking booking)
        {
            IRideService rideService = new RideService(configuration);
            Ride ride = rideService.GetRideById(booking.Ride.Id);
            ride.Id = booking.Ride.Id;
            int availableSeats = AvailableSeats(ride, booking.PickUp, booking.Drop);
            if (booking.Status.Value == "Rejected")
            {
                string query = "update Booking set PickUp=@PickUp,[Drop]=@Drop" +
                ",StatusId=@Status,NumberOfSeatsBooked=@NumberOfSeatsBooked where Id=@Id";
                DynamicParameters parameters = new DynamicParameters();
                string statusQuery = "select * from Status where Type='Booking' and Value='" + booking.Status.Value + "'";
                ExtensionObject statusExtensionObject = new ExtensionObject()
                {
                    Query = statusQuery,
                    ConnectionString = connectionString
                };
                string statusId = statusExtensionObject.GetItem<Status>().Id;

                parameters.Add("Id", booking.Id);
                parameters.Add("PickUp", booking.PickUp);
                parameters.Add("Drop", booking.Drop);
                parameters.Add("Status", statusId);
                parameters.Add("NumberOfSeatsBooked", booking.NumberOfSeatsBooked);
                ExtensionObject extensionObject = new ExtensionObject()
                {
                    Query = query,
                    ConnectionString = connectionString
                };
                //new { booking.Id, booking.PickUp, booking.Drop, booking.Status, booking.NumberOfSeatsBooked });
                if (extensionObject.AddOrUpdateItem<Booking>(parameters)) return 1;
                else return 0;
            }
            else if (booking.Status.Value=="Approved" && availableSeats >= booking.NumberOfSeatsBooked)
            {
                string query = "update Booking set PickUp=@PickUp,[Drop]=@Drop" +
                ",StatusId=@Status,NumberOfSeatsBooked=@NumberOfSeatsBooked where Id=@Id";
                DynamicParameters parameters = new DynamicParameters();
                string statusQuery = "select * from Status where Type='Booking' and Value='" + booking.Status.Value + "'";
                ExtensionObject statusExtensionObject = new ExtensionObject()
                {
                    Query = statusQuery,
                    ConnectionString = connectionString
                };
                string statusId = statusExtensionObject.GetItem<Status>().Id;

                parameters.Add("Id", booking.Id);
                parameters.Add("PickUp", booking.PickUp);
                parameters.Add("Drop", booking.Drop);
                parameters.Add("Status", statusId);
                parameters.Add("NumberOfSeatsBooked", booking.NumberOfSeatsBooked);
                ExtensionObject extensionObject = new ExtensionObject()
                {
                    Query = query,
                    ConnectionString = connectionString
                };
                //new { booking.Id, booking.PickUp, booking.Drop, booking.Status, booking.NumberOfSeatsBooked });
                if (extensionObject.AddOrUpdateItem<Booking>(parameters)) return 1;
                else return 0;
            }
            return -1;
        }

        public List<Booking> GetBookingsByRideId(string rideId)
        {
            //string query = "select * from Booking where RideId='" + rideId + "'";
            //return query.GetAllItems<Booking>();

            string query = "select b.Id,b.PickUp,b.[Drop],b.Price,b.NumberOfSeatsBooked,s.Value,customer.Name,customer.PhoneNumber,r.PickUp,r.Id,r.[Drop] " +
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
            string statusQuery = "select * from Status where Type='Booking' and Value='Cancelled'";
            ExtensionObject statusExtensionObject = new ExtensionObject()
            {
                Query = statusQuery,
                ConnectionString = connectionString
            };
            string statusId = statusExtensionObject.GetItem<Status>().Id;

            string query = "update Booking set StatusId=@statusId where RideId=@RideId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("RideId", rideId);
            parameters.Add("StatusId", statusId);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<Booking>(parameters);
        }

        public List<Booking> GetBookingsByUserId(string userId)
        {
            string query = "select b.Id,b.PickUp,b.[Drop],b.Price,b.NumberOfSeatsBooked,s.Value,u.Name,u.PhoneNumber,r.StartDate,r.PickUp,r.[Drop]" +
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
            Points.Add(ride.PickUp.ToLower());
            Seats.Add(0);

            IViaPointService viaPointService = new ViaPointService(configuration);
            List<ViaPoint> ViaPoints = viaPointService.GetAllViaPoints(ride.Id);
            int viaPointsCount = ViaPoints.Count;
            for(int i = 0; i < viaPointsCount; i++)
            {
                Points.Add("");
            }
            foreach(ViaPoint point in ViaPoints)
            {
                Points[point.Index] = point.Name;
                Seats.Add(0);
            }
            Points.Add(ride.Drop.ToLower());

            int fromIndex = Points.IndexOf(from.ToLower());
            int toIndex = Points.IndexOf(to.ToLower());
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
     