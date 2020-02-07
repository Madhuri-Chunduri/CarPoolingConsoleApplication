using CarPooling.Concerns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Contracts
{
    public interface IVehicleService
    {
        void AddVehicle(Vehicle vehicle);

        void DeleteVehicle(Vehicle vehicle);

        List<Vehicle> GetVehiclesByUserId(string userId);
    }
}
