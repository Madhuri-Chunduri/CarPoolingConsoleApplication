using CarPooling.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Interfaces
{
    public interface IVehicleService
    {
        void AddVehicle(Vehicle vehicle);

        void DeleteVehicle(Vehicle vehicle);

        List<Vehicle> GetVehiclesByUserId(string userId);
    }
}
