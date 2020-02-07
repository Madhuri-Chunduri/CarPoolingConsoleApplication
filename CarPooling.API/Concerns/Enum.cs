using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.API.Concerns
{
    public enum Gender
    {
        Female=0,
        Male
    }

    public enum RideStatus
    {
        NotStarted=0,
        Started,
        Cancelled,
        Finished
    }

    public enum BookingStatus
    {
        Approved=0,
        Pending,
        Cancelled,
        Rejected
    }

    public enum RideType
    {
        FromSource=0,
        FromViaPoint=1,
        ToViaPoint,
        BetweenViaPoints
    }
}
