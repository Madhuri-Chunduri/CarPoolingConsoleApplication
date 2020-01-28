using CarPooling.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Interfaces
{
    public interface IRideService
    {
        void AddRide(Ride ride);

        bool IsRidePointCovered(Ride ride, string ridePoint);

        List<Ride> GetAllRidesByUserId(string publisherId);

        List<Ride> FindRide(string from, string to);

        Ride GetRide(string id);

        void CancelRide(string id);

        void UpdateRide(Ride ride);

        int GetViaPointsCount(Ride ride);

        int GetIndex(Ride ride, string viaPoint);

        RideType GetRideType(Ride ride);
    }
}
