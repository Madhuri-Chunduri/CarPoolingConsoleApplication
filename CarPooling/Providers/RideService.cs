using CarPooling.Contracts;
using CarPooling.Concerns;
using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CarPooling.Providers
{
    public class RideService : IRideService
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

        public List<Ride> FindRide(string from, string to)
        {
            List<Ride> matchedRides = Rides.FindAll(obj => (obj.PickUp == from || obj.Drop == to) && obj.Status == 0);
            List<Ride> requiredRides = new List<Ride>();
            foreach (Ride ride in matchedRides)
            {
                int availableSeats = bookingService.AvailableSeats(ride, from, to);
                if (availableSeats > 0)
                {
                    ride.AvailableSeats = availableSeats;
                    requiredRides.Add(ride);
                }
            }

            matchedRides = new List<Ride>();
            List<Ride> unmatchedRides = Rides.FindAll(obj => (obj.PickUp != from || obj.Drop != to) && obj.Status == 0);
            foreach (Ride ride in unmatchedRides)
            {
                int availableSeats = 0;
                string viaPointFrom = ride.ViaPoints.FirstOrDefault(obj => obj == from || obj == ride.PickUp);
                string viaPointTo = ride.ViaPoints.FirstOrDefault(obj => obj == to || obj == ride.Drop);
                if (viaPointFrom != null && viaPointTo != null)
                {
                    availableSeats = bookingService.AvailableSeats(ride, viaPointFrom, viaPointTo);
                    if (availableSeats > 0)
                    {
                        ride.AvailableSeats = availableSeats;
                        requiredRides.Add(ride);
                    }
                }
                else if (viaPointFrom != null && ride.Drop == to)
                {
                    availableSeats = bookingService.AvailableSeats(ride, viaPointFrom, ride.Drop);
                    if (availableSeats > 0)
                    {
                        ride.AvailableSeats = availableSeats;
                        requiredRides.Add(ride);
                    }
                }
                else if (viaPointTo != null && ride.PickUp == from)
                {
                    availableSeats = bookingService.AvailableSeats(ride, ride.PickUp, viaPointTo);
                    if (availableSeats > 0)
                    {
                        ride.AvailableSeats = availableSeats;
                        requiredRides.Add(ride);
                    }
                }
            }
            return requiredRides;
        }

        public Ride GetRideById(string id)
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

        public void CancelRide(Ride ride)
        {
            Rides.Find(obj => obj.Id == ride.Id).Status = (RideStatus)2;
        }

        public void UpdateRide(Ride ride)
        {
            int oldRideIndex = Rides.IndexOf(ride);
            Rides[oldRideIndex] = ride;
        }
    }
}
