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
    public class StatusService:IStatusService
    {
        public IConfiguration configuration;
        public string connectionString;

        public StatusService(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetSection("Connectionstring").Value;
        }

        public Status GetStatus(string type,string value)
        {
            string query = "select * from Status where Type='" + type + "' and Value='" + value + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetItem<Status>();
        }

        public bool AddStatus(Status status)
        {
            string query = "insert into Status(Id,Type,Value) values(@Id,@Type,@Value)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id",Guid.NewGuid().ToString());
            parameters.Add("Type", status.Type);
            parameters.Add("Value", status.Value);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<Status>(parameters);
        }

        public bool UpdateStatus(Status status)
        {
            string query = "update Status set Type=@Type and Value=@Value where Id=@Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id",status.Id);
            parameters.Add("Type", status.Type);
            parameters.Add("Value", status.Value);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<Status>(parameters);
        }
    }
}
