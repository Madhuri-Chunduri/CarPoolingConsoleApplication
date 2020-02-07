using CarPooling.Concerns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Contracts
{
    public interface IRideService
    {
        void AddRide(Ride ride);

        List<Ride> GetAllRidesByUserId(string publisherId);

        List<Ride> FindRide(string from, string to);

        Ride GetRideById(string id);

        void UpdateRide(Ride ride);

        int GetViaPointsCount(Ride ride);

        int GetIndex(Ride ride, string viaPoint);
    }
}
