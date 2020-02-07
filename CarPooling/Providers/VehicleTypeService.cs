using CarPooling.Contracts;
using CarPooling.Concerns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Providers
{
    public class VehicleTypeService : IVehicleTypeService
    { 
        public static List<VehicleType> VehicleTypes = new List<VehicleType>();

        public bool AddVehicleType(VehicleType vehicleType)
        {
            try
            {
                VehicleTypes.Add(vehicleType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateVehicleType(VehicleType vehicleType)
        {
            try
            {
                int index = VehicleTypes.IndexOf(VehicleTypes.Find(obj => obj.Id == vehicleType.Id));
                VehicleTypes[index] = vehicleType;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<VehicleType> GetAllTypes()
        {
            try
            {
                return VehicleTypes;
            }
            catch
            {
                return null;
            }
        }
    }
}
