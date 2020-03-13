using CarPooling.API.Concerns;
using CarPooling.API.Contracts;
using Dapper;
using DapperExtensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CarPooling.API.Providers
{
    public class RideService :IRideService
    {
        string connectionString;
        public IConfiguration configuration;

        public RideService(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetSection("ConnectionString").Value;
        }

        public string AddRide(Ride ride)
        {
            string query = "insert into Ride(Id,PublisherId,PickUp,[Drop],StartDate,NumberOfSeats,Price,VehicleId,AutoApproveRide,StatusId) " +
                    "values(@Id,@PublisherId,@PickUp,@Drop,@StartDate,@NumberOfSeats,@Price,@VehicleId,@AutoApproveRide,@Status)";
            DynamicParameters parameters = new DynamicParameters();
            ride.Id = Guid.NewGuid().ToString();

            string statusQuery = "select * from Status where Type='Ride' and Value='Not Started'";
            ExtensionObject statusExtensionObject = new ExtensionObject()
            {
                Query = statusQuery,
                ConnectionString = connectionString
            };

            string rideStatusId = statusExtensionObject.GetItem<Status>().Id;
            ride.AutoApproveRide = false;

            parameters.Add("Id", ride.Id);
            parameters.Add("PublisherId", ride.Publisher.Id);
            parameters.Add("PickUp", ride.PickUp.ToLower());
            parameters.Add("Drop", ride.Drop.ToLower());
            parameters.Add("StartDate", ride.StartDate);
            parameters.Add("NumberOfSeats", ride.NumberOfSeats);
            //parameters.Add("AvailableSeats", ride.AvailableSeats);
            parameters.Add("Price", ride.Price);
            parameters.Add("VehicleId", ride.Vehicle.Id);
            parameters.Add("AutoApproveRide", ride.AutoApproveRide);
            parameters.Add("Status", rideStatusId);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            if (extensionObject.AddOrUpdateItem<Ride>(parameters)) return ride.Id;
            else return null;
        }

        public List<Ride> GetAllRidesByUserId(string publisherId)
        {
            string query = "select r.Id,r.PickUp,r.[Drop],r.StartDate,r.NumberOfSeats,r.Price,s.Value,v.Model,v.Id," +
                "v.Number,u.Name,u.PhoneNumber from Ride r inner join Vehicle v on r.VehicleId=v.Id " +
                "inner join Status s on s.Id=r.StatusId inner join [User] u on v.UserId=u.Id where r.PublisherId='" + publisherId + "'";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    return connection.Query<Ride, Status, Vehicle, User, Ride>(query, map: (ride, status, vehicle, publisher) =>
                     {
                         ride.Status = status;
                         ride.Vehicle = vehicle;
                         ride.Publisher = publisher;
                         return ride;
                     },
                   splitOn: "Value,Model,Name"
                   ).ToList();
                }
            }
            catch(Exception exception)
            {
                ILogService logService = new LogService(connectionString);
                logService.LogException(exception.GetType().ToString(), "GetAllRidesByUserId");
                return null;
            }
        }

        public List<Ride> FindRide(string from, string to)
        {
            from = from.ToLower();
            to = to.ToLower();
            List<Ride> requiredRides = new List<Ride>();
            int availableSeats = 0;

            IBookingService bookingService = new BookingService(configuration);
            IViaPointService viaPointService = new ViaPointService(configuration);

            string query = "select * from Ride";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };

            List<Ride> rides = extensionObject.GetAllItems<Ride>();

            foreach (Ride ride in rides)
            {
                availableSeats = bookingService.AvailableSeats(ride,from, to);
                if (availableSeats > 0)
                {
                    ride.AvailableSeats = availableSeats;
                    query = "select id,name,phoneNumber from [User] where Id= (select publisherId from Ride where Id='" + ride.Id+"')";
                    ExtensionObject publisherExtensionObject = new ExtensionObject()
                    {
                        Query = query,
                        ConnectionString = connectionString
                    };

                    ride.Publisher = publisherExtensionObject.GetItem<User>();
                    query = "select value from Status where Id=(select statusId from Ride where Id='" + ride.Id + "')";
                    ExtensionObject statusExtensionObject = new ExtensionObject()
                    {
                        Query = query,
                        ConnectionString = connectionString
                    };
                    ride.Status = statusExtensionObject.GetItem<Status>();
                    requiredRides.Add(ride);
                }
            }
            return requiredRides;
        }

        public Ride GetRideById(string id)
        {
            string query = "select r.PickUp,r.[Drop],r.StartDate,r.NumberOfSeats,r.Price,s.Value,v.Model," +
                "v.Number,u.Name,u.PhoneNumber from Ride r inner join Vehicle v on r.VehicleId=v.Id " +
                "inner join Status s on s.Id=r.StatusId inner join [User] u on r.PublisherId=u.Id where r.Id='" + id+"'";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    return connection.Query<Ride, Status,Vehicle, User, Ride>(query, map: (ride,status,vehicle, publisher) =>
                        {
                            ride.Status = status;
                            ride.Vehicle = vehicle;
                            ride.Publisher = publisher;
                            return ride;
                        },
                   splitOn: "Value,Model,Name"
                   ).FirstOrDefault();
                }
            }
            catch(Exception exception)
            {
                ILogService logService = new LogService(connectionString);
                logService.LogException(exception.GetType().ToString(), "GetRideById");
                return null;
            }
        }

        public bool UpdateRide(Ride ride)
        {
            if (ride.Status.Value == "Cancelled")
            {
                string statusQuery = "select * from Status where Type='Ride' and Value='Cancelled'";
                ExtensionObject statusExtensionObject = new ExtensionObject()
                {
                    Query = statusQuery,
                    ConnectionString = connectionString
                };

                string rideStatusId = statusExtensionObject.GetItem<Status>().Id;
                string query = "update Ride set StatusId=@StatusId where Id=@Id";
                ExtensionObject extensionObject = new ExtensionObject()
                {
                    Query = query,
                    ConnectionString = connectionString
                };
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", ride.Id);
                parameters.Add("StatusId", rideStatusId);
                if (extensionObject.AddOrUpdateItem<Ride>(parameters))
                {
                    IBookingService bookingService = new BookingService(configuration);
                    return bookingService.CancelAllBookingsByRideId(ride.Id);
                }
                return false;
            }
            else {
                string query = "update Ride set AutoApproveRide=@AutoApproveRide, VehicleId=@VehicleId, StatusId=@statusId where Id=@Id";
                string statusQuery = "select * from Status where Type='Ride' and Value='" + ride.Status.Value + "'";
                ExtensionObject statusExtensionObject = new ExtensionObject()
                {
                    Query = statusQuery,
                    ConnectionString = connectionString
                };
                string statusId = statusExtensionObject.GetItem<Status>().Id;

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", ride.Id);
                parameters.Add("AutoApproveRide", ride.AutoApproveRide);
                parameters.Add("VehicleId", ride.Vehicle.Id);
                parameters.Add("StatusId", statusId);
                ExtensionObject extensionObject = new ExtensionObject()
                {
                    Query = query,
                    ConnectionString = connectionString
                };
                return extensionObject.AddOrUpdateItem<Booking>(parameters);
            }
        }
    }
}
