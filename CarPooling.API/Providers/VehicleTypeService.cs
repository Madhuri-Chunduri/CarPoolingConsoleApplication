using CarPooling.API.Concerns;
using CarPooling.API.Contracts;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarPooling.API.Providers
{
    public class VehicleTypeService : IVehicleTypeService
    {
        public IConfiguration configuration;
        public string connectionString;

        public VehicleTypeService(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetSection("Connectionstring").Value;
        }


        public bool AddVehicleType(VehicleType vehicleType)
        {
            string query = "insert into VehicleType(Id,Name,MaximumFare,MaximumSeats) values(@Id,@Name,@MaximumFare,@MaximumSeats)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", Guid.NewGuid().ToString());
            parameters.Add("Name",vehicleType.Name);
            parameters.Add("MaximumFare",vehicleType.MaximumFare);
            parameters.Add("MaximumSeats", vehicleType.MaximumSeats);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<VehicleType>(parameters);
        }

        public List<VehicleType> GetAllVehicleTypes()
        {
            string query = "select * from VehicleType";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetAllItems<VehicleType>();
        }

        public bool UpdateVehicleType(VehicleType vehicleType)
        {
            string query = "update VehicleType set Name=@Name,MaximumFare=@MaximumFare,MaximumSeats=@MaximumSeats where Id=@Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", Guid.NewGuid().ToString());
            parameters.Add("Name", vehicleType.Name);
            parameters.Add("MaximumFare", vehicleType.MaximumFare);
            parameters.Add("MaximumSeats", vehicleType.MaximumSeats);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<VehicleType>(parameters);
        }
    }
}
