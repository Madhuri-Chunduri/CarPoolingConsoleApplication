using CarPooling.API.Concerns;
using System;
using System.Collections.Generic;
using System.Text;
using CarPooling.API.Contracts;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using DapperExtensions;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace CarPooling.API.Providers
{
    public class VehicleService : IVehicleService
    {
        public IConfiguration configuration;
        public string connectionString;

        public VehicleService(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetSection("Connectionstring").Value;
        }

        public bool AddVehicle(Vehicle vehicle)
        {
            string query = "insert into Vehicle(Id,UserId,Model,Number,VehicleTypeId,IsActive) values(@Id,@UserId,@Model,@Number,@vehicleTypeId,@IsActive)";
            vehicle.Id = Guid.NewGuid().ToString();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", vehicle.Id);
            parameters.Add("UserId", vehicle.UserId);
            parameters.Add("Model", vehicle.Model);
            parameters.Add("Number", vehicle.Number);
            parameters.Add("VehicleTypeId", vehicle.VehicleType.Id);
            parameters.Add("IsActive", vehicle.IsActive);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<Vehicle>(parameters);
        }

        public bool UpdateVehicle(Vehicle vehicle)
        {
            string query = "update Vehicle set Model=@Model,Number=@Number,VehicleTypeId=@vehicleTypeId,IsActive=@IsActive where Id=@Id)";
            vehicle.Id = Guid.NewGuid().ToString();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", vehicle.Id);
            parameters.Add("Model", vehicle.Model);
            parameters.Add("Number", vehicle.Number);
            parameters.Add("VehicleTypeId", vehicle.VehicleType.Id);
            parameters.Add("IsActive", vehicle.IsActive);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<Vehicle>(parameters);
        }

        public Vehicle GetVehicleById(string vehicleId)
        {
            string query = "select * from Vehicle where Id='" + vehicleId+"'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetItem<Vehicle>();
        }

        public List<Vehicle> GetVehiclesByUserId(string userId)
        {
            string query = "select * from Vehicle where UserId='" + userId + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetAllItems<Vehicle>();
        }
    }
}
