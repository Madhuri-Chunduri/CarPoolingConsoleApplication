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
    public class UserController : ControllerBase
    {
        IUserService userService { get; set; }

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("GetUserById/{id}")]
        public User GetUserById(string id)
        {
            return userService.GetUser(id);
        }

        [HttpGet]
        [Route("GetUserByMail/{email}")]
        public User GetUserByMail(string email)
        {
            return userService.GetUserByMail(email);
        }

        [HttpPost("AddPost")]
        public bool AddUser([FromBody] User user)
        {
            return userService.AddUser(user);
        }

        [HttpPut("UpdateUser")]
        public bool UpdateUser([FromBody] User user)
        {
           return userService.UpdateUser(user);
        }
    }
}