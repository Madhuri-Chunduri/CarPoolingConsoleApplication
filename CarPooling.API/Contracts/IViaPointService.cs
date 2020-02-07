using CarPooling.API.Concerns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.API.Contracts
{
    public interface IViaPointService
    {
        bool AddViaPoint(ViaPoint viaPoint);

        int GetViaPointsCount(Ride ride);

        int GetIndex(Ride ride, string viaPoint);

        bool IsViaPointExists(Ride ride, string viaPoint);

        List<string> GetAllViaPoints(string rideId);
    }
}
