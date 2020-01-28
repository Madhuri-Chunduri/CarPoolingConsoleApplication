using CarPooling.Interfaces;
using CarPooling.Models;
using CarPooling.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling
{
    class BookingActions
    {
        IBookingService bookingService = new BookingService();
        RideActions rideActions = new RideActions();
        IRideService rideService = new RideService();
        CommonMethods commonMethods = new CommonMethods();
        IUserService userService = new UserService();

        public void ViewOfferBookings()
        {
            List<Ride> offeredRides = rideService.GetAllRidesByUserId(LoginActions.currentUser.Id);
            int rideNumber;
            Console.Clear();
            if (offeredRides.Count == 0)
            {
                Console.WriteLine("You have not offered any rides!!");
            }
            else
            {
                rideNumber = 1;
                Console.WriteLine("Your offered rides are : ");
                Console.WriteLine("-----------------------------------------------------");
                foreach (Ride ride in offeredRides)
                {
                    double totalIncome = bookingService.GetTotalIncomeOfRide(ride);
                    Console.WriteLine(rideNumber + ". " + " From : " + ride.From + " To :  " + ride.To + " Total Seats : " + ride.NumberOfSeats+" Total Income : "+totalIncome);
                    Console.WriteLine(" Date of Ride : " + ride.Date.ToShortDateString() +  ride.Date.TimeOfDay + " Vehicle Name : " + ride.Vehicle.Name + " Ride Status : " + Enum.GetName(typeof(RideStatus), ride.Status));
                    rideNumber += 1;
                    Console.WriteLine("-----------------------------------------------------");
                }
                Console.WriteLine();
            }
            if (offeredRides.Count == 0)
            {
                commonMethods.FurtherAction(1);
            }

            rideNumber = commonMethods.ReadInt("Enter the ride number for which you want to view bookings : ");
            while (rideNumber < 1 || rideNumber > offeredRides.Count)
            {
                rideNumber = commonMethods.ReadInt("Please enter a valid choice between 1 and " + offeredRides.Count + " : ");
            }

            Ride selectedRide = offeredRides[rideNumber - 1];
            List<Booking> bookings = bookingService.GetBookingsByRideId(selectedRide.Id);
            int bookingNumber = 1;

            if(bookings.Count==0)
            {
                Console.WriteLine("There are no bookings yet to this ride!!");
                int furtherChoice = commonMethods.ReadInt("Do you want to continue viewing bookings ? 1. Yes 2. No >> ");
                furtherChoice = commonMethods.IsValidChoice(furtherChoice);
                if (furtherChoice == 1) ViewOfferBookings();
                else commonMethods.FurtherAction(1);
            }

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Serial Number   |"+" Booked By     |" + " Pick-Up | " + " Drop  |" + "  Fare | " + " Number of seats booked    |" + "Booking Status ");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------");
            foreach (Booking booking in bookings)
            {
                User Customer = userService.GetUser(booking.BookedBy);
                Console.WriteLine(bookingNumber + " \t\t|" + Customer.Name + " \t|  " + booking.From + " \t  |" + booking.To + "\t   |" + booking.Price + "\t   |" + booking.NumberOfSeatsBooked + "\t\t\t\t|" +
                   Enum.GetName(typeof(BookingStatus), booking.Status));
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------");
                bookingNumber += 1;
            }

            int decision = commonMethods.ReadInt("Enter the action you want to perform on bookings : 1. Approve Bookings 2. Reject Bookings 3. Go Back >> ");
            while (decision < 1 || decision > 3)
            {
                decision = commonMethods.ReadInt("Please enter a valid choice between 1 and 3 : ");
            }

            switch(decision)
            {
                case 1: int approveBookingNumber = commonMethods.ReadInt("Enter the booking number you want to approve : ");

                        while (approveBookingNumber < 1 || approveBookingNumber > bookings.Count)
                        {
                            approveBookingNumber = commonMethods.ReadInt("Please enter a valid choice between 1 and " + bookings.Count + " : ");
                        }

                        ApproveBooking(selectedRide,bookings[approveBookingNumber - 1]);
                        break;

                case 2: int rejectBookingNumber = commonMethods.ReadInt("Enter the booking number you want to reject : ");

                    while(rejectBookingNumber<1 || rejectBookingNumber>bookings.Count)
                    {
                        approveBookingNumber = commonMethods.ReadInt("Please enter a valid choice between 1 and " + bookings.Count + " : ");
                    }

                    bookingService.RejectBooking(bookings[rejectBookingNumber - 1].Id);

                    Console.WriteLine("Booking rejected successfully!!!");

                    break;
                case 3: commonMethods.FurtherAction(1);
                    break;
            }

            int choice = commonMethods.ReadInt("Do you want to continue viewing your offer bookings >> 1. Yes 2. No : ");
            choice = commonMethods.IsValidChoice(choice);
            if (choice == 1) ViewOfferBookings();
            else commonMethods.FurtherAction(1);
        }

        public void BookRide(Ride ride,string from,string to,int numberOfPassengers)
        {
            double price = 0;

            int viaPointsCount = rideService.GetViaPointsCount(ride);

            RideType rideType = GetRideType(ride,from,to);
            if (rideType == 0) price = ride.Price;
            else if(rideType==(RideType)1)
            {
                int fromIndex = rideService.GetIndex(ride,from);
                price = ((ride.Price)/(viaPointsCount+1))*(fromIndex + 1);
                price = Math.Round(price, MidpointRounding.AwayFromZero);
            }
            else if(rideType==(RideType)2)
            {
                int toIndex = rideService.GetIndex(ride,to);
                price = ((ride.Price) / (viaPointsCount+1)) * (toIndex + 1);
                price = Math.Round(price, MidpointRounding.AwayFromZero);
            }
            else
            {
                int fromIndex = rideService.GetIndex(ride,from);
                int toIndex = rideService.GetIndex(ride,to);
                price = ((ride.Price) / (viaPointsCount+1)) * (toIndex-fromIndex);
                price = Math.Round(price, MidpointRounding.AwayFromZero);
            }
            if (ride.PublisherId==LoginActions.currentUser.Id)
            {
                Console.WriteLine("Sorry!! You cannot book this ride");
                Console.WriteLine("Ride Publisher cannot book the ride");
                commonMethods.FurtherAction(1);
            }
            Booking booking = new Booking()
            {
                Id = Guid.NewGuid().ToString(),
                BookedBy = LoginActions.currentUser.Id,
                RideId = ride.Id,
                From = from,
                To = to,
                Price=price,
                NumberOfSeatsBooked = numberOfPassengers,
                Status = (ride.AutoApproveRide) ? (BookingStatus)0 : (BookingStatus)1,
            };
            bookingService.AddBooking(booking);
            Console.WriteLine("Booking Successful!!!");
            if (ride.AutoApproveRide == false) Console.WriteLine("You will get a notification as soon as the ride gets approved by the Publisher!!");
            commonMethods.FurtherAction(1);
        }

        public void ViewYourBookings()
        {
            List<Booking> bookings = bookingService.GetUserBookings();
            if (bookings.Count == 0)
            {
                Console.WriteLine("You have no booked rides!!!");
                commonMethods.FurtherAction(1);
            }
            if (bookings.Count == 1)
            {
                Booking booking = bookings[0];
                Ride bookedRide = rideService.GetRide(bookings[0].RideId);
                Console.WriteLine("You have booked only one ride!!");
                User Publisher = userService.GetUser(bookedRide.PublisherId);

                Console.WriteLine(" Publisher Name : " + Publisher.Name + " | Total Booked Seats : " + booking.NumberOfSeatsBooked + " | Pick- up : "
                    + booking.From + " | Drop : " + booking.To + " | Fare : " + booking.Price);
                Console.WriteLine("   Time of Ride : " + bookedRide.Date.ToShortDateString() + bookedRide.Date.TimeOfDay + " | Vehicle Name : " + bookedRide.Vehicle.Name + " | Ride Status : "
                    + Enum.GetName(typeof(RideStatus), bookedRide.Status) + " | Booking Status : " + Enum.GetName(typeof(BookingStatus), booking.Status));
                Console.WriteLine("-------------------------------------------------------------------------------------------------");

                int choice = commonMethods.ReadInt("Do you want to cancel the above booking ? 1. Yes 2. No >> ");
                choice = commonMethods.IsValidChoice(choice);
                if (choice == 1) CancelBooking(booking);
                else commonMethods.FurtherAction(1);
            }

            int rideNumber = 1;

            Console.WriteLine("S.No | Publisher Name  |" + " Booked Seats  |" + " Pick-up \t|" + " Drop" + "\t\t|" + " Fare |" + " Date Of Ride  \t|" + " Vehicle \t|" + " Ride Status \t|" + " Booking Status");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------");

            foreach (Booking booking in bookings)
            {
                Ride bookedRide = bookingService.GetRideById(booking.RideId);
                User Publisher = userService.GetUser(bookedRide.PublisherId);
                Console.WriteLine(rideNumber + "   |" + Publisher.Name + "\t\t|" + booking.NumberOfSeatsBooked + " \t\t|"
                    + booking.From + " \t| " + booking.To + " \t| " + booking.Price + " \t|"
                    + bookedRide.Date.ToShortDateString() + bookedRide.Date.TimeOfDay + " | " + bookedRide.Vehicle.Name + ""
                    + " \t|" + Enum.GetName(typeof(RideStatus), bookedRide.Status) + " \t| " + Enum.GetName(typeof(BookingStatus), booking.Status));
                Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------");
                rideNumber += 1;
            }

            int decision = commonMethods.ReadInt("Do you want to cancel/modify any booking ? 1. Modify 2. Cancel 3. Go Back >> ");
            decision = commonMethods.IsValidChoice(decision);
            while (decision < 1 || decision > 3)
            {
                decision = commonMethods.ReadInt("Please enter a valid value between 1 and 3 : ");
            }

            if (decision == 1)
            {
                if (bookings.Count == 1)
                {
                    int choice = commonMethods.ReadInt("Do you want to modify the above booking ? 1. Yes 2. Go Back >> ");
                    choice = commonMethods.IsValidChoice(choice);
                    if (choice == 1) UpdateBooking(bookings[0]);
                    else commonMethods.FurtherAction(1);
                }
                else
                {
                    int updationChoice = commonMethods.ReadInt("Enter the booking number you want to update : ");
                    while (updationChoice < 1 || updationChoice > bookings.Count)
                    {
                        updationChoice = commonMethods.ReadInt("Please enter a valid choice between 1 and " + bookings.Count + " : ");
                    }
                    UpdateBooking(bookings[updationChoice - 1]);
                }
            }

            else if (decision == 2)
            {
                if (bookings.Count == 1)
                {
                    int choice = commonMethods.ReadInt("Do you want to cancel the above booking ? 1. Yes 2. Go Back >> ");
                    choice = commonMethods.IsValidChoice(choice);
                    if (choice == 1) CancelBooking(bookings[0]);
                    else commonMethods.FurtherAction(1);
                }
                else
                {
                    int cancellationChoice = commonMethods.ReadInt("Enter the booking number you want to cancel : ");
                    while (cancellationChoice < 1 || cancellationChoice > bookings.Count)
                    {
                        cancellationChoice = commonMethods.ReadInt("Please enter a valid choice between 1 and " + bookings.Count + " : ");
                    }
                    CancelBooking(bookings[cancellationChoice - 1]);
                }
            }
            else commonMethods.FurtherAction(1);
        }

        public RideType GetRideType(Ride ride,string from,string to)
        {
            if (ride.From == from)
            {
                if (ride.To == to) return 0;
                else return (RideType)2;
            }
            else if (ride.To == to)
            {
                return (RideType)1;
            }
            else return (RideType)3;
        }

        public void CancelBooking(Booking booking)
        {
            if (booking.Status == 0 || booking.Status == (BookingStatus)1)
            {
                bookingService.CancelBookingById(booking.Id);
                Console.WriteLine("Booking Cancelled  Successfully!!");
                commonMethods.FurtherAction(1);
            }
            else
            {
                Console.WriteLine(" The booking is already cancelled!!");
                int furtherChoice = commonMethods.ReadInt("Do you want to continue using the application >> 1. Yes 2. Logout");
                furtherChoice = commonMethods.IsValidChoice(furtherChoice);
                commonMethods.FurtherAction(furtherChoice);
            }
        }

        public void UpdateBooking(Booking booking)
        {
            int decision = commonMethods.ReadInt("Enter the field you want to update : 1. From 2. To 3. Number of seats 4. Go Back >> ");
            while(decision<1 || decision>4)
            {
                decision = commonMethods.ReadInt("Please enter a valid choice between 1 and 4 : ");
            }
            Ride ride = rideService.GetRide(booking.RideId);

            switch(decision)
            {
                case 1: string newFrom = commonMethods.ReadString("Enter the modified Pick-up point : ");
                    bool isFromExists = rideService.IsRidePointCovered(ride,newFrom);
                    if(isFromExists==true)
                    {
                        BookingStatus bookingStatus = booking.Status;
                        bookingService.CancelBookingById(booking.Id);
                        int availableSeats = GetAvailableSeats(ride, booking);
                        if(availableSeats<booking.NumberOfSeatsBooked)
                        {
                            Console.WriteLine("There are not enough seats between " + newFrom + " and " + booking.To);
                            Console.WriteLine("You cannot update this booking!!");
                            booking.Status = bookingStatus;
                            bookingService.UpdateBooking(booking);
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            ViewYourBookings();
                        }
                        else
                        {
                            booking.From = newFrom;
                            if (ride.AutoApproveRide == false) booking.Status = (BookingStatus)1;
                            else booking.Status = bookingStatus;
                            bookingService.UpdateBooking(booking);
                            Console.WriteLine("Booking updated successfully!!");
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            ViewYourBookings();
                        }
                    }
                    else
                    {
                        Console.WriteLine("This ride does not cover " + newFrom + "..");
                        Console.WriteLine("This booking cannot be updated!!!");
                        commonMethods.FurtherAction(1);
                    }
                    break;

                case 2:
                    string newTo = commonMethods.ReadString("Enter the modified drop point : ");
                    bool isToExists = rideService.IsRidePointCovered(ride,newTo);
                    if (isToExists == true)
                    {
                        BookingStatus bookingStatus = booking.Status;
                        bookingService.CancelBookingById(booking.Id);
                        int availableSeats = GetAvailableSeats(ride, booking);
                        if (availableSeats < booking.NumberOfSeatsBooked)
                        {
                            Console.WriteLine("There are not enough seats between " + booking.From + " and " + newTo);
                            Console.WriteLine("You cannot update this booking!!");
                            booking.Status = bookingStatus;
                            bookingService.UpdateBooking(booking);
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            ViewYourBookings();
                        }
                        else
                        {
                            booking.To = newTo;
                            if (ride.AutoApproveRide == false) booking.Status = (BookingStatus)1;
                            else booking.Status = bookingStatus;
                            bookingService.UpdateBooking(booking);
                            Console.WriteLine("Booking updated successfully!!");
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            ViewYourBookings();
                        }
                    }
                    else
                    {
                        Console.WriteLine("This ride does not cover " + newTo + "..");
                        Console.WriteLine("This booking cannot be updated!!!");
                        commonMethods.FurtherAction(1);
                    }
                    break;

                case 3: Console.WriteLine("Currently you have booked " + booking.NumberOfSeatsBooked + " seats.");
                    int newNumberOfSeats = commonMethods.ReadInt("Enter the new number of seats you want to book : ");
                    while(newNumberOfSeats<0)
                    {
                        Console.WriteLine("New number of seats cannot be less than 0");
                        newNumberOfSeats = commonMethods.ReadInt("Please enter a valid value : ");
                    }
                    if(newNumberOfSeats==0)
                    {
                        decision = commonMethods.ReadInt("Do you want to cancel this booking ? 1. Yes 2. No : ");
                        decision = commonMethods.IsValidChoice(decision);
                        if (decision == 1) CancelBooking(booking);
                        else commonMethods.FurtherAction(1);
                    }
                    else
                    {
                        BookingStatus bookingStatus = booking.Status;
                        bookingService.CancelBookingById(booking.Id);
                        int availableSeats = GetAvailableSeats(ride, booking);
                        if (availableSeats < newNumberOfSeats)
                        {
                            Console.WriteLine("There are not enough seats between " + booking.From + " and " + booking.To);
                            Console.WriteLine("You cannot update this booking!!");
                            booking.Status= bookingStatus;
                            bookingService.UpdateBooking(booking);
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            ViewYourBookings();
                        }
                        else
                        {
                            booking.NumberOfSeatsBooked = newNumberOfSeats;
                            if (ride.AutoApproveRide == false) booking.Status = (BookingStatus)1;
                            else booking.Status = bookingStatus;
                            bookingService.UpdateBooking(booking);
                            Console.WriteLine("Booking updated successfully!!");
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            ViewYourBookings();
                        }
                    }
                    break;

                case 4: commonMethods.FurtherAction(1);
                    break;
            }
        }

        public void ApproveBooking(Ride ride, Booking booking)
        {
            int availableSeats = GetAvailableSeats(ride, booking);

            if (booking.Status == 0)
            {
                Console.WriteLine("The booking is already approved!!");
            }
            else
            {
                if (availableSeats == 0)
                {
                    Console.WriteLine("You cannot approve this ride as all the seats are already full");
                    commonMethods.FurtherAction(1);
                }
                else
                {
                    if (ride.Status == (RideStatus)0 || ride.Status == (RideStatus)1)
                    {
                        if (availableSeats >= booking.NumberOfSeatsBooked)
                        {
                            bookingService.ApproveBooking(booking.Id);
                            Console.WriteLine("Booking is approved!!");
                        }
                        else
                        {
                            Console.WriteLine("Sorry!!! The ride does not have" + booking.NumberOfSeatsBooked + " seats!!");
                            Console.WriteLine("This booking cannot be approved..");
                        }
                    }
                    else
                    {
                        if (ride.Status == (RideStatus)2) Console.WriteLine("Booking cannot be approved as the ride was cancelled!!");
                        if (ride.Status == (RideStatus)3) Console.WriteLine("Booking cannot be approved as the ride is already finished!!");
                    }
                }
            }
        }

        public int GetAvailableSeats(Ride ride, Booking booking)
        {
            RideType rideType = GetRideType(ride, booking.From, booking.To);

            int availableSeats = 0;

            switch (rideType)
            {
                case 0:
                    availableSeats = bookingService.AreSeatsAvailable(ride);
                    break;
                case (RideType)1:
                    availableSeats = bookingService.AreSeatsAvailableFromViaPoint(ride, booking.From);
                    break;
                case (RideType)2:
                    availableSeats = bookingService.AreSeatsAvailableToViaPoint(ride, booking.To);
                    break;
                case (RideType)3:
                    availableSeats = bookingService.AreSeatsAvailableBetweenViaPoints(ride, booking.From, booking.To);
                    break;
            }
            return availableSeats;
        }

}
}
 