using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarPooling.API.Concerns;
using CarPooling.API.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarPooling.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleTypeController : ControllerBase
    {
        IVehicleTypeService vehicleTypeService;

        public VehicleTypeController(IVehicleTypeService vehicleTypeService)
        {
            this.vehicleTypeService = vehicleTypeService;
        }

        [HttpGet("GetAllVehicleTypes")]
        public List<VehicleType> GetAllVehicleTypes()
        {
            return vehicleTypeService.GetAllVehicleTypes();
        }

        [HttpPost("AddVehicleType")]
        public bool AddVehicleType([FromBody] VehicleType vehicleType)
        {
            return vehicleTypeService.AddVehicleType(vehicleType);
        }

        [HttpPut("UpdateVehicleType")]
        public bool UpdateVehicleType([FromBody] VehicleType vehicleType)
        {
            return vehicleTypeService.UpdateVehicleType(vehicleType);
        }
    }
}