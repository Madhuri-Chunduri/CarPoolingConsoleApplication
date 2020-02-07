using CarPooling.Concerns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Contracts
{
    public interface IVehicleTypeService
    {
        bool AddVehicleType(VehicleType vehicleType);

        bool UpdateVehicleType(VehicleType vehicleType);

        List<VehicleType> GetAllTypes();
    }
}
