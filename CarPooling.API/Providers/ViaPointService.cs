using CarPooling.API.Contracts;
using CarPooling.API.Concerns;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using DapperExtensions;
using Microsoft.Extensions.Configuration;

namespace CarPooling.API.Providers
{
    public class ViaPointService : IViaPointService
    {
        public IConfiguration configuration;
        public string connectionString;

        public ViaPointService(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetSection("ConnectionString").Value;
        }


        public bool AddViaPoint(ViaPoint viaPoint)
        {
            string query = "insert into ViaPoint(Id,RideId,Name,[Index],Distance) values(@Id,@RideId,@Name,@Index,@Distance)";
            DynamicParameters parameters = new DynamicParameters();
            string viaPointId = Guid.NewGuid().ToString();
            parameters.Add("Id", viaPointId);
            parameters.Add("RideId", viaPoint.RideId);
            parameters.Add("Name", viaPoint.Name.ToLower());
            parameters.Add("Index", viaPoint.Index);
            parameters.Add("Distance", viaPoint.Distance);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            if (extensionObject.AddOrUpdateItem<ViaPoint>(parameters))
                return true;
            else return false;
        }

        public List<ViaPoint> GetAllViaPoints(string rideId)
        {
            string query = "select * from ViaPoint where RideId='" + rideId + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetAllItems<ViaPoint>();
        }

        public int GetIndex(Ride ride, string viaPoint)
        {
            string query = "select Name from ViaPoint where RideId='" + ride.Id + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetAllItems<string>().IndexOf(viaPoint);
        }

        public int GetViaPointsCount(Ride ride)
        {
            string query = "select * from ViaPoint where RideId='" + ride.Id + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetAllItems<ViaPoint>().Count();
        }

        public bool IsViaPointExists(Ride ride, string viaPoint)
        {
            string query = "select Name from ViaPoint where RideId='" + ride.Id + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetAllItems<string>().Contains(viaPoint);
        }
    }
}
