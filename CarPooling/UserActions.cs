using CarPooling.Interfaces;
using CarPooling.Models;
using CarPooling.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling
{
    public class UserActions
    {
        CommonMethods commonMethods = new CommonMethods();
        RideActions rideActions = new RideActions();
        VehicleActions vehicleActions = new VehicleActions();
        BookingActions bookingActions = new BookingActions();
        IUserService userService = new UserService();

        public void DisplayMethods()
        {
            Console.Clear();

            List<string> Actions = new List<string> { "Offer a ride", "Find a ride", "View your offered rides","View your bookings",
                "View bookings of your offered rides","View your vehicles ","Update Profile","LogOut"};

            int actionNumber = 1;
            foreach(string action in Actions)
            {
                Console.WriteLine(actionNumber + ". " + action);
                actionNumber += 1;
            }

            int choice = commonMethods.ReadInt("Enter your choice : ");
            if(choice<0 || choice > actionNumber)
            {
                Console.WriteLine("Sorry!! Invalid Choice ");
                DisplayMethods();
            }

            switch(choice)
            {
                case 1: rideActions.OfferRide();
                    break;

                case 2: rideActions.FindARide();
                    break;

                case 3: rideActions.ViewOfferedRides();
                    break;

                case 4: bookingActions.ViewYourBookings();
                    break;

                case 5: bookingActions.ViewOfferBookings();
                    break;

                case 6: vehicleActions.ViewVehicles();
                    break;

                case 7: UpdateProfile();
                    break;

                case 8: LoginActions loginActions = new LoginActions();
                    loginActions.LogOut();
                    break;
            }
        }

        public void UpdateProfile()
        {
            Console.WriteLine("Enter the field you want to update : ");
            Console.WriteLine("1. Name 2. Mobile Number 3. Address 4. Email 5. Password 6. Exit");
            int choice = commonMethods.ReadInt("Your choice : ");
            User user = LoginActions.currentUser;
            User updatedUser = new User();
            switch (choice)
            {
                case 1:
                    Console.WriteLine("Your old name : " + LoginActions.currentUser.Name);
                    user.Name = commonMethods.ReadString("Please enter new name : ");
                    while (user.Name == "")
                    {
                        Console.WriteLine("Please enter a valid name");
                        user.Name = commonMethods.ReadString("Enter new name : ");
                    }
                    updatedUser = userService.UpdateUser(user);
                    break;
                case 2:
                    Console.WriteLine("Your old mobile number : " + LoginActions.currentUser.PhoneNumber);
                    user.PhoneNumber = commonMethods.ReadString("Please enter new mobile number : ");
                    bool validNumber = commonMethods.IsValidPhoneNumber(user.PhoneNumber);
                    while (validNumber != true)
                    {
                        Console.WriteLine("Sorry!! This is not a valid mobile number..");
                        user.PhoneNumber = commonMethods.ReadString("Enter valid Mobile Number :");
                        validNumber = commonMethods.IsValidPhoneNumber(user.PhoneNumber);
                    }
                    updatedUser = userService.UpdateUser(user);
                    break;
                case 3:
                    Console.WriteLine("Your old address : " + LoginActions.currentUser.Address);
                    user.Address = commonMethods.ReadString("Please enter new address : ");
                    updatedUser = userService.UpdateUser(user);
                    break;
                case 4:
                    Console.WriteLine("Your old email : " + LoginActions.currentUser.Email);
                    user.Email = commonMethods.ReadString("Please enter new email : ");
                    bool validEmail = commonMethods.IsValidEmail(user.Email);
                    while (validEmail == false)
                    {
                        user.Email = commonMethods.ReadString("Enter valid Email Address : ");
                        validEmail = commonMethods.IsValidEmail(user.Email);
                    }
                    updatedUser = userService.UpdateUser(user);
                    break;
                case 5:
                    Console.WriteLine("Your old password : " + LoginActions.currentUser.Password);
                    user.Password = commonMethods.ReadString("Please enter new password : ");
                    while (user.Password == "")
                    {
                        user.Password = commonMethods.ReadString("Please enter a valid password : ");
                    }
                    updatedUser = userService.UpdateUser(user);
                    break;
                case 6:
                    DisplayMethods();
                    break;
            }
            Console.WriteLine("Your current details are : ");
            Console.WriteLine(" Name : " + updatedUser.Name);
            Console.WriteLine(" Mobile Number : " + updatedUser.PhoneNumber);
            Console.WriteLine(" Address : " + updatedUser.Address);
            Console.WriteLine(" Email : " + updatedUser.Email);
            Console.WriteLine(" Password : " + updatedUser.Password);
            commonMethods.FurtherAction(1);
        }
    }
}

