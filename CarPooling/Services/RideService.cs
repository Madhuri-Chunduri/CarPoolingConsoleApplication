using CarPooling.Interfaces;
using CarPooling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarPooling.Services
{
    public class RideService :IRideService
    {
        public static List<Ride> Rides = new List<Ride>();
        IBookingService bookingService = new BookingService();

        public void AddRide(Ride ride)
        {
            Rides.Add(ride);
        }

        public List<Ride> GetAllRidesByUserId(string publisherId)
        {
            return Rides.FindAll(obj => obj.PublisherId == publisherId);
        }

        public List<Ride> FindRide(string from,string to)
        {
            List<Ride> matchedRides = Rides.FindAll(obj => obj.From == from && obj.To == to && obj.Status==0);
            List<Ride> requiredRides = new List<Ride>();
            foreach(Ride ride in matchedRides)
            {
                int availableSeats = bookingService.AreSeatsAvailable(ride);
                if (availableSeats > 0)
                {
                    ride.AvailableSeats = availableSeats;
                    requiredRides.Add(ride);
                }
            }

            matchedRides = new List<Ride>();
            List<Ride> unmatchedRides = Rides.FindAll(obj => (obj.From != from || obj.To != to) && obj.Status==0);
            foreach (Ride ride in unmatchedRides)
            {
                int availableSeats = 0;
                string viaPointFrom = ride.ViaPoints.FirstOrDefault(obj => obj == from || obj == ride.From);
                string viaPointTo = ride.ViaPoints.FirstOrDefault(obj => obj == to || obj == ride.To);
                if (viaPointFrom != null && viaPointTo != null)
                {
                    availableSeats = bookingService.AreSeatsAvailableBetweenViaPoints(ride, viaPointFrom, viaPointTo);
                    if (availableSeats > 0)
                    {
                        ride.AvailableSeats = availableSeats;
                        requiredRides.Add(ride);
                    }
                }
                else if (viaPointFrom != null && ride.To == to)
                {
                    availableSeats = bookingService.AreSeatsAvailableFromViaPoint(ride, viaPointFrom);
                    if (availableSeats>0)
                    {
                        ride.AvailableSeats = availableSeats;
                        requiredRides.Add(ride);
                    }
                }
                else if (viaPointTo != null && ride.From == from)
                {
                    availableSeats = bookingService.AreSeatsAvailableToViaPoint(ride, viaPointTo);
                    if (availableSeats>0)
                    {
                        ride.AvailableSeats = availableSeats;
                        requiredRides.Add(ride);
                    }
                }
            }
            return requiredRides;
        }

        public Ride GetRide(string id)
        {
            return Rides.Find(obj => obj.Id == id);
        }

        public int GetViaPointsCount(Ride ride)
        {
            return Rides.Find(obj => obj.Id == ride.Id).ViaPoints.Count();
        }

        public int GetIndex(Ride ride, string viaPoint)
        {
            return Rides.Find(obj => obj.Id == ride.Id).ViaPoints.IndexOf(viaPoint);
        }

        public void CancelRide(string id)
        {
            Rides.Find(obj => obj.Id == id).Status = (RideStatus)2;
        }

        public void UpdateRide(Ride ride)
        {
            int oldRideIndex = Rides.IndexOf(ride);
            Rides[oldRideIndex] = ride;
        }

        public bool IsRidePointCovered(Ride ride, string ridePoint)
        {
            if (ride.From == ridePoint || ride.To == ridePoint) return true;
            if (ride.ViaPoints.Contains(ridePoint)) return true;
            return false;
        }

        public RideType GetRideType(Ride ride)
        {
            Ride matchedRide = Rides.FirstOrDefault(obj => obj.From == ride.From && obj.To == ride.To);
            if(matchedRide==null)
            {
                matchedRide = Rides.FirstOrDefault(obj => obj.From == ride.From && obj.Id==ride.Id);
                if (matchedRide != null) return (RideType)1;
                matchedRide = Rides.FirstOrDefault(obj => obj.To == ride.To && obj.Id == ride.Id);
                if (matchedRide != null) return (RideType)2;
                else return (RideType)3;
            }
            return (RideType)0;
        }
    }
}
