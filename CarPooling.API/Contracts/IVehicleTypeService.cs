using CarPooling.API.Concerns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarPooling.API.Contracts
{
    public interface IVehicleTypeService
    {
        List<VehicleType> GetAllVehicleTypes();

        bool AddVehicleType(VehicleType vehicleType);

        bool UpdateVehicleType(VehicleType vehicleType);
    }
}
