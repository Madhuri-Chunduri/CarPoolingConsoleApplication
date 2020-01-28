using CarPooling.Models;
using System;
using System.Collections.Generic;
using System.Text;
using CarPooling.Interfaces;
using System.Linq;

namespace CarPooling.Services
{
    public class VehicleService : IVehicleService
    {
        public static List<Vehicle> Vehicles = new List<Vehicle>();

        public void AddVehicle(Vehicle vehicle)
        {
            Vehicles.Add(vehicle);
        }

        public void DeleteVehicle(Vehicle vehicle)
        {
            Vehicles.Remove(vehicle);
        }

        public List<Vehicle> GetVehiclesByUserId(string userId)
        {
            return Vehicles.FindAll(obj => obj.UserId == userId);
        }
    }
}
