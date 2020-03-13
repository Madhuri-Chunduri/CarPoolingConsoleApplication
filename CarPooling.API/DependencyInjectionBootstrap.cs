using CarPooling.API.Contracts;
using CarPooling.API.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace CarPooling.API
{
    public class DependencyInjectionBootstrap
    {
        public static void InjectedServices(IConfiguration Configuration, IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRideService, RideService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IVehicleTypeService, VehicleTypeService>();
            services.AddScoped<IViaPointService, ViaPointService>();
            services.AddScoped<IStatusService,StatusService>();
        }
    }
}
