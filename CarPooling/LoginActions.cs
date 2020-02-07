using CarPooling.Contracts;
using CarPooling.Concerns;
using CarPooling.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling
{
    public class LoginActions
    {
        CommonMethods commonMethods = new CommonMethods();
        IUserService userService = new UserService();

        public static User currentUser = new User();

        public void Register()
        {
            Console.Clear();

            Console.WriteLine("<------REGISTER------>");
            User user = new User();
            user.Email = commonMethods.ReadString("Enter your email : ");
            bool isValidEmail = commonMethods.IsValidEmail(user.Email);
            while (!isValidEmail)
            {
                user.Email = commonMethods.ReadString("Please enter a valid email : ");
                isValidEmail = commonMethods.IsValidEmail(user.Email);
            }
            bool isExistingUser = userService.IsExistingUser(user.Email);
            if(isExistingUser)
            {
                Console.WriteLine("You already have an account with us!!");
                int Choice = commonMethods.ReadInt("Do you want to login to that account : 1. Yes 2. No >> ");
                TakeAction(Choice);
            }

            user.Id = Guid.NewGuid().ToString();
            user.Name = commonMethods.ReadString("Enter your name : ");
            
            user.PhoneNumber = commonMethods.ReadString("Enter your Phone Number : ");
            bool isValidPhoneNumber = commonMethods.IsValidPhoneNumber(user.PhoneNumber);
            while (!isValidPhoneNumber)
            {
                user.PhoneNumber = commonMethods.ReadString("Please enter a valid phone number : ");
                isValidPhoneNumber = commonMethods.IsValidPhoneNumber(user.PhoneNumber);
            }
            user.Address = commonMethods.ReadString("Enter your address : ");
            int gender = commonMethods.ReadInt("Please choose your Gender : 1. Female 2. Male >> ");
            if (gender == 1) user.Gender = (Gender)0;
            else user.Gender = (Gender)1;
            user.Password = commonMethods.ReadString("Enter your Password : ");
            string reEnterPassword = commonMethods.ReadString("Re-enter your password : ");
            while(user.Password!=reEnterPassword)
            {
                Console.WriteLine("Your password and re-enter password did not match!!");
                Console.WriteLine("Re-enter your password");
                user.Password = commonMethods.ReadString("Enter your Password : ");
                reEnterPassword = commonMethods.ReadString("Re-enter your password : ");
            }
            userService.AddUser(user);

            int addVehicle = commonMethods.ReadInt("Do you want to add your vehicle : 1. Yes 2. No >> ");
            if (addVehicle == 1)
            {
                IVehicleService vehicleService = new VehicleService();
                Vehicle vehicle = new Vehicle();
                vehicle.UserId = user.Id;
                vehicle.Model = commonMethods.ReadString("Enter your Vehicle Model : ");
                vehicle.Number = commonMethods.ReadString("Enter your Vehicle Number : ");
                vehicleService.AddVehicle(vehicle);
            }

            currentUser = user;
            Console.WriteLine("Registration successful!!");
            commonMethods.FurtherAction(1);
        }

        public void LogIn()
        {
            Console.Clear();

            Console.WriteLine("<------LOGIN------>");
            string email = commonMethods.ReadString("Enter your registered mail id : ");
            bool isValidEmail = commonMethods.IsValidEmail(email);
            while (!isValidEmail)
            {
                email = commonMethods.ReadString("Please enter a valid email : ");
                isValidEmail = commonMethods.IsValidEmail(email);
            }

            bool isExistingUser = userService.IsExistingUser(email);
            if (!isExistingUser)
            {
                Console.WriteLine("You don't have an account with us!!");
                int Choice = commonMethods.ReadInt("Do you want to register an account with us : 1. Yes 2. No >> ");
                while (Choice < 1 || Choice > 3) Choice = commonMethods.ReadInt("Please enter a choice between 1 and 2 : ");
                if (Choice == 1) Register();
                else LogIn();
            }

            string password = commonMethods.ReadString("Enter your password : ");
            bool isValidUser = userService.IsValidUser(email, password);
            if (isValidUser == false)
            {
                Console.WriteLine("Sorry!! Invalid Credentials..");
                int choice = commonMethods.ReadInt("Do you want to continue : 1. Login 2. Register >> ");
                TakeAction(choice);
            }
            else
            {
                currentUser = userService.GetUserByMail(email);
                commonMethods.FurtherAction(1);
            }
        }

        public void LogOut()
        {
            currentUser = null;
            Console.Clear();
            int choice = commonMethods.ReadInt("Enter your choice : 1.Login 2. Register >> ");
            TakeAction(choice);
        }

        public void TakeAction(int choice)
        {
            while (choice > 3 || choice < 0)
            {
                choice = commonMethods.ReadInt("Please enter a valid choice between 1 and 2 : ");
            }
            if (choice == 1) LogIn();
            else if (choice == 2) Register();
        }

    }
}
