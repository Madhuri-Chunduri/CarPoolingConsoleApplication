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
    public class BookingController : ControllerBase
    {
        IBookingService bookingService { get; set; }

        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpGet]
        [HttpGet("GetBookingsByRideId/{rideId}")]
        public List<Booking> GetBookingsByRideId(string rideId)
        {
            return bookingService.GetBookingsByRideId(rideId);
        }

        [Route("GetBookingsByUserId/{userId}")]
        [HttpGet]
        public List<Booking> GetBookingsByUserId(string userId)
        {
            return bookingService.GetBookingsByUserId(userId);
        }

        [HttpPost("AddBooking")]
        public bool AddBooking(Booking booking)
        {
           return bookingService.AddBooking(booking);
        }

        [HttpPut("UpdateBooking")]
        public bool UpdateBooking(Booking booking)
        {
            return bookingService.UpdateBooking(booking);
        }
    }
}