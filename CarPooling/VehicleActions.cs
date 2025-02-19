﻿using CarPooling.Concerns;
using CarPooling.Contracts;
using CarPooling.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling
{
    public class VehicleActions
    {
        CommonMethods commonMethods = new CommonMethods();
        IVehicleService vehicleService = new VehicleService();
        IRideService rideService = new RideService();

        public Vehicle AddVehicle()
        {
            Vehicle vehicle = new Vehicle();
            vehicle.Id = Guid.NewGuid().ToString();
            vehicle.UserId = LoginActions.currentUser.Id;
            vehicle.Model = commonMethods.ReadString("Enter the model of the vehicle : ");
            vehicle.Number = commonMethods.ReadString("Enter the number of the vehicle : ");
            vehicle.IsActive = true;
            vehicleService.AddVehicle(vehicle);
            return vehicle;
        }

        public void ViewVehicles()
        {
            List<Vehicle> vehicles = vehicleService.GetVehiclesByUserId(LoginActions.currentUser.Id);

            if (vehicles.Count == 0)
            {
                Console.WriteLine("You have not added any vehicles!!");
                int addVehicleChoice = commonMethods.ReadInt("Do you want to add a new vehicle ? 1. Yes 2. Go Back >> ");
                addVehicleChoice = commonMethods.IsValidChoice(addVehicleChoice);
                if (addVehicleChoice == 1)
                {
                    AddVehicle();
                    ViewVehicles();
                }
                else commonMethods.FurtherAction(1);
            }

            Console.WriteLine("Your vehicles are : ");
            Console.WriteLine("---------------------------------------------------------");
            int vehicleCount = 1;
            Console.WriteLine("Serial Number    | Vehicle Name          | Vehicle Number");
            Console.WriteLine("---------------------------------------------------------");
            foreach (Vehicle vehicle in vehicles)
            {
                Console.WriteLine(vehicleCount + "\t" + "\t" + " | " + vehicle.Model + "\t" + "\t" + " | " + vehicle.Number);
                vehicleCount += 1;
            }

            Console.WriteLine("---------------------------------------------------------");
            int actionChoice = commonMethods.ReadInt("Enter the choice you want to perform : 1. Add new vehicle 2. Delete a vehicle 3. Go Back >> ");
            while (actionChoice < 1 || actionChoice > 4)
            {
                actionChoice = commonMethods.ReadInt("Please enter a valid choice between 1 and 3 : ");
            }
            switch (actionChoice)
            {
                case 1:
                    AddVehicle();
                    Console.WriteLine("Vehicle added successfully!!!");
                    commonMethods.FurtherAction(1);
                    break;

                case 2:
                    if (vehicles.Count == 0)
                    {
                        Console.WriteLine("You have not added any vehicles!!");
                        commonMethods.FurtherAction(1);
                    }
                    if (vehicles.Count == 1)
                    {
                        int decision = commonMethods.ReadInt("Are you sure you want to delete the above vehicle ?? 1. Yes 2. No >> ");
                        decision = commonMethods.IsValidChoice(decision);
                        if (decision == 1)
                        {
                            vehicles[0].IsActive = false;
                            vehicleService.DeleteVehicle(vehicles[0]);
                        }
                    }
                    else
                    {
                        int vehicleChoice = commonMethods.ReadInt("Enter the serial number of vehicle you want to delete :  ");
                        while (vehicleChoice < 1 || vehicleChoice > vehicles.Count)
                        {
                            vehicleChoice = commonMethods.ReadInt("Please enter a valid value between 1 and " + vehicles.Count + " : ");
                        }
                        vehicles[vehicleChoice - 1].IsActive = false;
                        vehicleService.DeleteVehicle(vehicles[vehicleChoice - 1]);
                    }
                    commonMethods.FurtherAction(1);
                    break;

                case 3:
                    commonMethods.FurtherAction(1);
                    break;
            }
        }

        public void UpdateVehicleForRide(Ride ride)
        {
            Vehicle currentVehicle = ride.Vehicle;
            Console.WriteLine("Current details of the vehicle are : ");
            Console.WriteLine("Vehicle Model : " + currentVehicle.Model);

            // Console.WriteLine("Vehicle Number : " + ride.Vehicle.Number);
            int choice = commonMethods.ReadInt("Are you sure to update the vehicle for this ride : 1. Yes 2. Go Back >> ");
            choice = commonMethods.IsValidChoice(choice);
            if (choice == 1)
            {
                List<Vehicle> vehicles = vehicleService.GetVehiclesByUserId(LoginActions.currentUser.Id);

                Console.WriteLine("Your vehicles are : ");
                Console.WriteLine("-------------------------------------");
                int vehicleCount = 1;
                Console.WriteLine("Serial Number /t | Vehicle Model /t | Vehicle Number");
                foreach (Vehicle vehicle in vehicles)
                {
                    Console.WriteLine(vehicleCount + "/t" + " | " + vehicle.Model + "/t" + " | " + vehicle.Number);
                    vehicleCount += 1;
                }

                choice = commonMethods.ReadInt("Do you want to use any of the above vehicles ? 1. Yes 2. No >> ");
                choice = commonMethods.IsValidChoice(choice);

                if (choice == 1)
                {
                    int vehicleChoice = commonMethods.ReadInt("Enter the vehicle serial number you want to use for this ride : ");
                    while (vehicleChoice < 1 || vehicleChoice > vehicles.Count)
                    {
                        vehicleChoice = commonMethods.ReadInt("Please enter a valid choice : ");
                    }
                    ride.Vehicle = vehicles[vehicleChoice - 1];
                }
                else
                {
                    ride.Vehicle = AddVehicle();
                }
                rideService.UpdateRide(ride);
            }
        }
    }
}
