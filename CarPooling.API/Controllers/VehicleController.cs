using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarPooling.API.Contracts;
using CarPooling.API.Concerns;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarPooling.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        IVehicleService vehicleService { get; set; }

        public VehicleController(IVehicleService vehicleService)
        {
            this.vehicleService = vehicleService;
        }

        [HttpGet("GetVehicleById/{id}")]
        public Vehicle GetVehicleById(string id)
        {
            return vehicleService.GetVehicleById(id);
        }

        [HttpGet]
        [Route("GetVehiclesByUserId/{id}")]
        public List<Vehicle> GetVehiclesByUserId(string id)
        {
            return vehicleService.GetVehiclesByUserId(id);
        }

        [HttpPost("AddVehicle")]
        public string AddVehicle([FromBody] Vehicle vehicle)
        {
            return vehicleService.AddVehicle(vehicle); 
        }
    }
}