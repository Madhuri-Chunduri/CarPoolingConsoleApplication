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
using System.Threading.Tasks;

namespace CarPooling.API.Providers
{
    public static class UtilityService
    {
        public static List<T> GetAllItems<T>(this ExtensionObject extensionObject)
        {
            try
            {
                using (var connection = new SqlConnection(extensionObject.ConnectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    return connection.Query<T>(extensionObject.Query).ToList();
                }
            }
            catch(Exception exception)
            {
                LogService logService = new LogService(extensionObject.ConnectionString);
                logService.LogException(exception.GetType().ToString(), "GetAll" + typeof(T).Name.ToString());
                return null;
            }
        }

        public static T GetItem<T>(this ExtensionObject extensionObject)
        {
            try
            {
                using (var connection = new SqlConnection(extensionObject.ConnectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    return connection.Query<T>(extensionObject.Query).FirstOrDefault();
                }
            }
            catch(Exception exception)
            {
                LogService logService = new LogService(extensionObject.ConnectionString);
                logService.LogException(exception.GetType().ToString(),"Get"+ typeof(T).Name.ToString());
                return default(T);
            }
        }

        public static bool AddOrUpdateItem<T>(this ExtensionObject extensionObject, DynamicParameters parameters)
        {
            try
            {
                using (var connection = new SqlConnection(extensionObject.ConnectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    connection.Execute(extensionObject.Query, parameters);
                    return true;
                }
            }
            catch(Exception exception)
            {
                LogService logService = new LogService(extensionObject.ConnectionString);
                logService.LogException(exception.GetType().ToString(), "Add" + typeof(T).Name.ToString());
                return false;
            }
        }
    }
}
