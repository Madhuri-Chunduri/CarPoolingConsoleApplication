using CarPooling.Concerns;
using CarPooling.Contracts;
using CarPooling.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling
{
    public class RideActions
    {
        CommonMethods commonMethods = new CommonMethods();
        VehicleActions vehicleActions = new VehicleActions();
        IRideService rideService =new RideService();
        IVehicleService vehicleService = new VehicleService();
        IUserService userService = new UserService();
        IBookingService bookingService = new BookingService();
        IVehicleTypeService vehicleTypeService = new VehicleTypeService();

        public void FindARide()
        {
            string from = commonMethods.ReadString("Enter your source : ");
            string to = commonMethods.ReadString("Enter your destination : ");
            DateTime inputtedDate = ReadDateTime();
           
            List<Ride> matchedRides = rideService.FindRide(from, to);
            if(matchedRides.Count==0)
            {
                Console.WriteLine("Sorry!! There are no rides in your way!!");
                commonMethods.FurtherAction(1);
            }
            int rideNumber = 1;
            Console.WriteLine("Serial Number   |" + " Ride Publisher |" + "  Total Seats | "+" Available Seats | "+ " Source \t| " + "  Destination | " + " Date of Ride \t| " + " Vehicle Name  ");
            foreach (Ride ride in matchedRides)
            {
                if(ride.Date<inputtedDate)
                {
                    continue;
                }

                User publisher = userService.GetUser(ride.PublisherId);

                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine(rideNumber + "\t\t|" + publisher.Name + "    \t|" +ride.NumberOfSeats+"\t\t|"+ride.AvailableSeats+"\t\t|"+ride.PickUp+" \t|"+ride.Drop+"\t|"+ride.Date.ToShortDateString().ToString()
                    +" "+ride.Date.TimeOfDay.ToString()+"\t|"+ride.Vehicle.Model);
                rideNumber += 1;
            }

            if(rideNumber==1)
            {
                Console.WriteLine("Sorry!! There are no rides in your way!!");
                commonMethods.FurtherAction(1);
            }
            Console.WriteLine("Matched Rides : " + (rideNumber-1));
          
            int choice = commonMethods.ReadInt("Do you want to book a ride ? 1. Yes 2. No >> ");
            choice = commonMethods.IsValidChoice(choice);
            BookingActions bookingActions = new BookingActions();
            if (choice == 1)
            {
                if(matchedRides.Count==1)
                {
                    int bookingDecision = commonMethods.ReadInt("Are you sure you want to book the above ride ? 1. Yes 2. No >> ");
                    bookingDecision = commonMethods.IsValidChoice(bookingDecision);
                    if (bookingDecision == 1)
                    {
                        int numberOfPassengers = commonMethods.ReadInt("Enter number of passengers : ");
                        while (numberOfPassengers < 1 || numberOfPassengers > 4)
                        {
                            if (numberOfPassengers < 1) numberOfPassengers = commonMethods.ReadInt("Passengers cannot be less than 1");
                            else numberOfPassengers = commonMethods.ReadInt("Passengers cannot be more than 4 : ");
                        }
                        while(numberOfPassengers>matchedRides[0].AvailableSeats)
                        {
                            Console.WriteLine("There are only " + matchedRides[0].AvailableSeats + " available for this ride!!");
                            int decision = commonMethods.ReadInt("Do you want to continue booking by reducing passengers : 1. Yes 2. No >> ");
                            decision = commonMethods.IsValidChoice(decision);
                            if (decision == 1) numberOfPassengers = commonMethods.ReadInt("Please enter number of passengers less than " + matchedRides[0].AvailableSeats + " : ");
                            else commonMethods.FurtherAction(1);
                        }
                        bookingActions.BookRide(matchedRides[0], from, to, numberOfPassengers);
                    }
                    else commonMethods.FurtherAction(1);
                }
                else
                {
                    int bookingChoice = commonMethods.ReadInt("Enter the ride number you want to book (1 to " + matchedRides.Count + ") : ");
                    while (bookingChoice < 1 || bookingChoice > rideNumber)
                    {
                        bookingChoice = commonMethods.ReadInt("Please enter a valid choice between 1 and " + matchedRides.Count);
                    }
                    int numberOfPassengers = commonMethods.ReadInt("Enter number of passengers : ");
                    while (numberOfPassengers < 1 || numberOfPassengers > 4)
                    {
                        if (numberOfPassengers < 1) numberOfPassengers = commonMethods.ReadInt("Passengers cannot be less than 1");
                        else numberOfPassengers = commonMethods.ReadInt("Passengers cannot be more than 4 : ");
                    }
                    while (numberOfPassengers > matchedRides[bookingChoice-1].AvailableSeats)
                    {
                        Console.WriteLine("There are only " + matchedRides[bookingChoice - 1].AvailableSeats + " available for this ride!!");
                        int decision = commonMethods.ReadInt("Do you want to continue booking by reducing passengers : 1. Yes 2. No >> ");
                        decision = commonMethods.IsValidChoice(decision);
                        if (decision == 1) numberOfPassengers = commonMethods.ReadInt("Please enter number of passengers less than " + matchedRides[bookingChoice - 1].AvailableSeats + " : ");
                        else commonMethods.FurtherAction(1);
                    }
                    bookingActions.BookRide(matchedRides[bookingChoice - 1],from,to,numberOfPassengers);
                }
                Console.WriteLine("Booking successful!!");
            }
            else commonMethods.FurtherAction(1);
        }

        public void OfferRide()
        {
            int choice = commonMethods.ReadInt("Are you sure you want to offer a ride? >> 1. Yes 2. No : ");
            choice = commonMethods.IsValidChoice(choice);
            if(choice==2)
            {
                commonMethods.FurtherAction(1);
            }
            Ride ride = new Ride();
            ride.Id = Guid.NewGuid().ToString();
            ride.PickUp = commonMethods.ReadString("Enter your source of ride : ");
            ride.Drop = commonMethods.ReadString("Enter your destination of ride : ");
            ride.Date = ReadDateTime();
            ride.PublisherId = LoginActions.currentUser.Id;

            List<VehicleType> vehicleTypes = vehicleTypeService.GetAllTypes();
            int count = 0;
            Console.WriteLine("Serial Number | Type \t | Maximum Fare | Maximum Seats");
            foreach (VehicleType vehicleType in vehicleTypes)
            {
                ++count;
                Console.WriteLine(count+"\t"+vehicleType.Type + "" + vehicleType.MaximumFare + "\t" + vehicleType.MaximumSeats);
            }

            int vehicleTypeChoice = commonMethods.ReadInt("Choose your vehicle type from above types : ");
            while(vehicleTypeChoice<0 || vehicleTypeChoice>count)
            {
                vehicleTypeChoice = commonMethods.ReadInt("Please enter a valid choice : ");
            }
            ride.VehicleType = vehicleTypes[vehicleTypeChoice - 1];
            ride.NumberOfSeats = commonMethods.ReadInt("Enter the number of seats available for the ride : ");
            while(ride.NumberOfSeats> ride.VehicleType.MaximumSeats || ride.NumberOfSeats < 1)
            {
                if (ride.NumberOfSeats < 1) Console.WriteLine("There should be atleast one seat in the ride");
                else Console.WriteLine("You cannot have more than "+ ride.VehicleType.MaximumSeats+" seats");
                ride.NumberOfSeats = commonMethods.ReadInt(" Please enter valid number of seats : ");
            }
            ride.AvailableSeats = ride.NumberOfSeats;
            int autoApprove = commonMethods.ReadInt("Auto-approve the bookings ? 1. Yes 2. No >> ");
            autoApprove = commonMethods.IsValidChoice(autoApprove);
            ride.AutoApproveRide = (autoApprove == 1) ? true : false;

            ride.Price = commonMethods.ReadDouble("Enter the price of the ride : ");
            while(ride.Price>ride.VehicleType.MaximumFare)
            {
                Console.WriteLine("Fare for the ride should not be greater than " + ride.VehicleType.MaximumFare);
                ride.Price = commonMethods.ReadDouble("Please enter a valid price : ");
            }

            ride.Status = (RideStatus)0;

            List<Vehicle> vehicles = vehicleService.GetVehiclesByUserId(LoginActions.currentUser.Id);
            if (vehicles.Count == 1)
            {
                Console.WriteLine(" Vehicle Model : " + vehicles[0].Model);
                choice = commonMethods.ReadInt("Do you want to offer ride with above vehicle : 1. Yes 2. No >> ");
                choice = commonMethods.IsValidChoice(choice);
                if (choice == 1) ride.Vehicle = vehicles[0];
                else ride.Vehicle = vehicleActions.AddVehicle();
            }
            else if (vehicles.Count == 0)
            {
                ride.Vehicle = vehicleActions.AddVehicle();
            }
            else
            {
                int optionNumber = 1;
                foreach (Vehicle vehicle in vehicles)
                    Console.WriteLine(optionNumber + ". Vehicle Model : " + vehicle.Model);
                choice = commonMethods.ReadInt("Do you want to use any of the above vehicles : 1. Yes 2. No >> ");
                choice = commonMethods.IsValidChoice(choice);
                if (choice == 1)
                {
                    int vehicleChoice = commonMethods.ReadInt("Enter the vehicle number you want to choose : ");
                    while (vehicleChoice < 1 || vehicleChoice > optionNumber)
                    {
                        vehicleChoice = commonMethods.ReadInt("Please enter a valid option : ");
                    }
                    ride.Vehicle.Id = vehicles[optionNumber - 1].Id;
                }
                else
                {
                    Console.WriteLine("Enter your new vehicle data");
                    ride.Vehicle = vehicleActions.AddVehicle();
                }
            }
            rideService.AddRide(ride);

            int viaPointsChoice = commonMethods.ReadInt("Do you want to add Via Points ? 1.Yes 2. No >> ");
            viaPointsChoice = commonMethods.IsValidChoice(viaPointsChoice);
            int viaPointCount = 1;
            string viaPoint;
            ride.ViaPoints = new List<string>();

            while (viaPointsChoice == 1)
            {
                viaPoint = commonMethods.ReadString("Via Point " + viaPointCount + " : ");
                if (ride.ViaPoints.Contains(viaPoint))
                {
                    Console.WriteLine("You have already added this via point!!");
                }
                else ride.ViaPoints.Add(viaPoint);
                viaPointsChoice = commonMethods.ReadInt("Add one more Via Point ? 1.Yes 2. No >> ");
                viaPointsChoice = commonMethods.IsValidChoice(viaPointsChoice);
                viaPointCount += 1;
            }

            
            Console.WriteLine("Ride added successfully!!");
            commonMethods.FurtherAction(1);
        }

        public void ViewOfferedRides()
        {
            List<Ride> offeredRides = rideService.GetAllRidesByUserId(LoginActions.currentUser.Id);
            if (offeredRides.Count == 0)
            {
                Console.WriteLine("You have not offered any rides!!");
            }
            else
            {
                int rideNumber = 1;
                Console.WriteLine("Your offered rides are : ");
                Console.WriteLine("-----------------------------------------------------");
                foreach (Ride ride in offeredRides)
                {
                    double totalIncome = bookingService.GetTotalIncomeOfRide(ride);
                    Console.WriteLine(rideNumber + ". " + " From : " + ride.PickUp + " To :  " + ride.Drop + " Total Seats : " + ride.NumberOfSeats + " Total Income : " + totalIncome);
                    Console.WriteLine(" Date of Ride : " + ride.Date.ToShortDateString() + " Time of Ride : " + ride.Date.TimeOfDay
                        + " Vehicle Model : " + ride.Vehicle.Model);
                    Console.WriteLine(" Ride Status : " +Enum.GetName(typeof(RideStatus),ride.Status));
                    rideNumber += 1;
                    Console.WriteLine("-----------------------------------------------------");
                }
                Console.WriteLine();
                int rideActionChoice = commonMethods.ReadInt("Do you want to update/cancel any rides ? 1. Update ride 2. Cancel ride 3. Go Back >> ");
                while(rideActionChoice<1 || rideActionChoice>3)
                {
                    rideActionChoice = commonMethods.ReadInt("Please enter a valid choice between 1 and 3 : ");
                }
                if (rideActionChoice == 1)
                {
                    int rideChoice = commonMethods.ReadInt("Enter the ride number which you want to update : ");
                    while (rideChoice < 1 || rideChoice > offeredRides.Count)
                    {
                        rideChoice = commonMethods.ReadInt("Please enter a valid choice : ");
                    }
                    UpdateRide(offeredRides[rideChoice - 1]);
                }
                else if (rideActionChoice == 2)
                {
                    int rideChoice = commonMethods.ReadInt("Enter the ride number which you want to cancel : ");
                    while (rideChoice < 1 || rideChoice > offeredRides.Count)
                    {
                        rideChoice = commonMethods.ReadInt("Please enter a valid choice : ");
                    }
                    CancelRide(offeredRides[rideChoice - 1]);
                }
                else commonMethods.FurtherAction(1);
            }
        }

        public void CancelRide(Ride ride)
        {
            ride.Status = (RideStatus)2;
            rideService.UpdateRide(ride);
            bookingService.CancelAllBookingsByRideId(ride.Id);
            Console.WriteLine("Ride deleted successfully!!!");
            commonMethods.FurtherAction(1);
        }

        public void UpdateRide(Ride ride)
        {
            Console.WriteLine("Enter the field number you want to update");
            int updateFieldNumber = commonMethods.ReadInt("1. Vehicle  2. Auto-Approval Type >> ");
            updateFieldNumber = commonMethods.IsValidChoice(updateFieldNumber);
            switch(updateFieldNumber)
            {
                case 1: int decision = commonMethods.ReadInt("Are you sure you want to change the vehicle ?? 1. Yes 2. No, Go Back >> ");
                    decision = commonMethods.IsValidChoice(decision);
                    if (decision == 1)
                    {
                        vehicleActions.UpdateVehicleForRide(ride);
                        Console.WriteLine("Vehicle for this ride is updated successfully!!");
                    }
                    else commonMethods.FurtherAction(1);
                    break;

                case 2: Console.WriteLine("Currently Auto-Approval is set to : " + ride.AutoApproveRide);
                    int approvalDecision = commonMethods.ReadInt("Do you want to update the Auto-Approval status to : " + !(ride.AutoApproveRide)+ " >> !. Yes 2. No");
                    approvalDecision = commonMethods.IsValidChoice(approvalDecision);
                    ride.AutoApproveRide = !ride.AutoApproveRide;
                    if (approvalDecision == 1)
                    {
                        rideService.UpdateRide(ride);
                        List<Booking> bookings = bookingService.GetBookingsByRideId(ride.Id);
                        foreach (Booking booking in bookings)
                        {
                            if (booking.Status == (BookingStatus)2)
                            {
                                booking.Status = (BookingStatus)1;
                                bookingService.UpdateBooking(booking);
                            }
                        }
                        //Notify customer with approval of the above ride
                        Console.WriteLine("Auto-Approval status updated successfully!!!");
                    }
                    commonMethods.FurtherAction(1);
                    break;
            }
        }

        public DateTime ReadDateTime()
        {
            Console.Write("Please enter Date and Time in D / M / YYYY HH: MM : ");
            DateTime dateTime;
            while(!DateTime.TryParseExact(Console.ReadLine(),"d/M/yyyy H:mm",null,System.Globalization.DateTimeStyles.None,out dateTime))
            {
                Console.Write("Please enter Date DD/MM/YYYY HH:MM format : ");
            }

            while(dateTime<DateTime.Now)
            {
                Console.WriteLine("Date and time cannot be less than today's date and time!!");
                dateTime = ReadDateTime();
            }
            return dateTime;
        }
    }
}
