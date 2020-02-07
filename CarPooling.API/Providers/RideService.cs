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

        public bool AddRide(Ride ride)
        {
            string query = "insert into Ride(Id,PublisherId,PickUp,[Drop],StartDate,NumberOfSeats,AvailableSeats,Price,VehicleId,AutoApproveRide,Status) " +
                    "values(@Id,@PublisherId,@PickUp,@Drop,@StartDate,@NumberOfSeats,@AvailableSeats,@Price,@VehicleId,@AutoApproveRide,@Status)";
            DynamicParameters parameters = new DynamicParameters();
            ride.Id = Guid.NewGuid().ToString();
            parameters.Add("Id", ride.Id);
            parameters.Add("PublisherId", ride.Publisher.Id);
            parameters.Add("PickUp", ride.PickUp);
            parameters.Add("Drop", ride.Drop);
            parameters.Add("StartDate", ride.StartDate);
            parameters.Add("NumberOfSeats", ride.NumberOfSeats);
            parameters.Add("AvailableSeats", ride.AvailableSeats);
            parameters.Add("Price", ride.Price);
            parameters.Add("VehicleId", ride.Vehicle.Id);
            parameters.Add("AutoApproveRide", ride.AutoApproveRide);
            parameters.Add("Status", ride.Status.Id);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<Ride>(parameters);
        }

        public List<Ride> GetAllRidesByUserId(string publisherId)
        {
            string query = "select r.PickUp,r.[Drop],r.StartDate,r.NumberOfSeats,r.Price,s.Value,v.Model," +
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
            string query = "update Ride set AutoApproveRide=@AutoApproveRide, VehicleId=@VehicleId where Id=@Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", ride.Id);
            parameters.Add("AutoApproveRide", ride.AutoApproveRide);
            parameters.Add("VehicleId", ride.Vehicle.Id);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<Booking>(parameters);
        }
    }
}
