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
    public class LogService : ILogService
    {
        public string connectionString;

        public LogService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool LogException(string exception, string methodName)
        {
            string query = "insert into Log(Id,Method,Exception,TimeStamp) values(@Id,@Method,@Exception,@TimeStamp)";
           
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", Guid.NewGuid().ToString());
            parameters.Add("Method", methodName);
            parameters.Add("Exception", exception);
            parameters.Add("Timestamp", DateTime.Now);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<Log>(parameters);
        }

        public List<Log> GetAllLogItems()
        {
            string query = "select * from Log";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetAllItems<Log>();
        }
    }
}
