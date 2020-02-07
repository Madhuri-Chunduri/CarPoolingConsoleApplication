using CarPooling.API.Concerns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.API.Contracts
{
    public interface IVehicleService
    {
        bool AddVehicle(Vehicle vehicle);

        bool UpdateVehicle(Vehicle vehicle);

        Vehicle GetVehicleById(string vehicleId);

        List<Vehicle> GetVehiclesByUserId(string userId);
    }
}
