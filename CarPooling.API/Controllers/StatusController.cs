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
    public class StatusController : ControllerBase
    {
        IStatusService statusService;

        public StatusController(IStatusService statusService)
        {
            this.statusService = statusService;
        }

        [HttpGet("GetStatus")]
        public Status GetStatus([FromQuery] string type,[FromQuery] string value)
        {
            return this.statusService.GetStatus(type,value);
        }

        [HttpPost("AddStatus")]
        public bool AddStatus([FromBody] Status status)
        {
            return this.statusService.AddStatus(status);
        }

        [HttpPut("UpdateStatus")]
        public bool UpdateStatus(Status status)
        {
            return this.statusService.UpdateStatus(status);
        }
        
    }
}