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
    public class RideController : ControllerBase
    {
        IRideService rideService { get; set; }

        public RideController(IRideService rideService)
        {
            this.rideService = rideService;
        }

        [HttpGet("GetRideById/{id}")]
        public Ride GetRideById(string id)
        {
            return rideService.GetRideById(id);
        }

        [HttpPost("AddRide")]
        public string AddRide([FromBody] Ride ride)
        {
             return rideService.AddRide(ride);
        }

        [HttpGet]
        [Route("GetAllRidesByUserId/{id}")]
        public List<Ride> GetAllRidesByUserId(string id)
        {
            return rideService.GetAllRidesByUserId(id);
        }

        [HttpGet]
        [Route("FindRide")]
        public List<Ride> FindRide([FromQuery] string from,[FromQuery] string to)
        {
            return rideService.FindRide(from, to);
        }

        [HttpPut("UpdateRide")]
        public bool UpdateRide(Ride ride)
        {  
            return rideService.UpdateRide(ride);
        }
    }
}