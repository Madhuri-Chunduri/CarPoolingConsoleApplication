using CarPooling.API.Concerns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.API.Contracts
{
    public interface IRideService
    {
        bool AddRide(Ride ride);

        List<Ride> GetAllRidesByUserId(string publisherId);

        List<Ride> FindRide(string from, string to);

        Ride GetRideById(string id);
        
        bool UpdateRide(Ride ride);
    }
}
