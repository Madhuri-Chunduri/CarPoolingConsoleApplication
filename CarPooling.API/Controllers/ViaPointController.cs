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
    public class ViaPointController : ControllerBase
    {
        IViaPointService viaPointService;

        public ViaPointController(IViaPointService viaPointService)
        {
            this.viaPointService = viaPointService;
        }

        [HttpPost("AddViaPoint")]
        public bool AddViaPoint([FromBody] ViaPoint viaPoint)
        {
            return viaPointService.AddViaPoint(viaPoint);
        }
    }
}