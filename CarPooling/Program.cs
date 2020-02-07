using CarPooling.Concerns;
using CarPooling.Contracts;
using CarPooling.Providers;
using System;

namespace CarPooling
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonMethods commonMethods = new CommonMethods();
            LoginActions loginActions = new LoginActions();
            IVehicleTypeService vehicleTypeService = new VehicleTypeService();
            VehicleType newVehicleType = new VehicleType()
            {
                Id = Guid.NewGuid().ToString(),
                Type = "Four-Wheeler",
                MaximumFare = 20,
                MaximumSeats = 4
            };

            vehicleTypeService.AddVehicleType(newVehicleType);
            int choice = commonMethods.ReadInt("Already have an account ? 1. Login 2. Register >> ");

            while(choice<1 || choice>3)
            {
                choice = commonMethods.ReadInt("Please enter a valid choice");
            }

            if (choice == 1) loginActions.LogIn();
            else loginActions.Register();
        }
    }
}
