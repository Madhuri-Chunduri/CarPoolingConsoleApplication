using CarPooling.Concerns;
using CarPooling.Contracts;
using CarPooling.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling
{
    public class VehicleTypeActions
    {
        CommonMethods commonMethods = new CommonMethods();
        IVehicleTypeService vehicleTypeService = new VehicleTypeService();

        public void AddVehicleType()
        {
            string type = commonMethods.ReadString("Enter the type of vehicle : ");
            bool isTypeAlreadyExists = IsTypeAlreadyExists(type);
            if(isTypeAlreadyExists)
            {
                Console.WriteLine("This vehicle type already exists!!");
                int decision = commonMethods.ReadInt("Do you want to update the type "+type+"  : 1. Yes 2. Go Back >> ");
                decision = commonMethods.IsValidChoice(decision);
                if (decision == 1)  UpdateVehicleType();
                else commonMethods.FurtherAction(1);
            }
            else
            {
                double maximumFare = commonMethods.ReadDouble("Enter the maximum fare allowed for " + type + " : ");
                while(maximumFare<1)
                {
                    maximumFare = commonMethods.ReadDouble("Please enter valid fare value : ");
                }
                int maximumSeats = commonMethods.ReadInt("Enter the maximum number of seats allowed for " + type + " : ");
                while(maximumSeats<1)
                {
                    maximumSeats = commonMethods.ReadInt("Please enter valid number of seats : ");
                }

                VehicleType vehicleType = new VehicleType()
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    MaximumFare = maximumFare,
                    MaximumSeats = maximumSeats
                };
                 vehicleTypeService.AddVehicleType(vehicleType);
                commonMethods.FurtherAction(1);
            }
        }

        public void UpdateVehicleType()
        {
            List<VehicleType> vehicleTypes = vehicleTypeService.GetAllTypes();
            int count = 1;
            if (vehicleTypes.Count > 0)
            {
                Console.WriteLine("Serial Number | Type \t | Maximum Fare | Maximum Seats");
                
                foreach (VehicleType vehicleType in vehicleTypes)
                {
                    Console.WriteLine(count+"\t"+vehicleType.Type + "" + vehicleType.MaximumFare + "\t" + vehicleType.MaximumSeats);
                    count++;
                }
            }
            else
            {
                Console.WriteLine("There are no vehicle types to update!!!");
                int decision = commonMethods.ReadInt("Do you want to add a type ? 1. Yes 2. Go Back >> ");
                decision = commonMethods.IsValidChoice(decision);
                if (decision == 1) AddVehicleType();
                else commonMethods.FurtherAction(1);
            }
            count--;
            int index = commonMethods.ReadInt("Enter the vehicle type you want to update : ");
            while(index>count || index<0)
            {
                index = commonMethods.ReadInt("Please enter a valid choice : ");
            }
            int choice = commonMethods.ReadInt("Choose the field which you want to update : 1. Maximum Fare 2. Maximum Seats 3. Go Back : ");
            while(choice<1 || choice>3)
            {
                choice = commonMethods.ReadInt("Please enter a valid choice between 1 and 3 : ");
            }
            switch(choice)
            {
                case 1: Console.WriteLine("Current Fare : " + vehicleTypes[index - 1].MaximumFare);
                    vehicleTypes[index-1].MaximumFare = commonMethods.ReadDouble("Please enter the new maximum fare : ");
                    while(vehicleTypes[index - 1].MaximumFare<0)
                    {
                        vehicleTypes[index - 1].MaximumFare = commonMethods.ReadDouble("Please enter a valid fare greater than 0 : ");
                    }
                    bool status = vehicleTypeService.UpdateVehicleType(vehicleTypes[index - 1]);
                    if(status==true)
                    {
                        Console.WriteLine("Vehicle Type updated successfully!!");
                        commonMethods.FurtherAction(1);
                    }
                    else
                    {
                        Console.WriteLine("There is an error updating the vehicle type!!");
                        commonMethods.FurtherAction(1);
                    }
                    break;

                case 2: Console.WriteLine("Current Maximum Seats : " + vehicleTypes[index - 1].MaximumSeats);
                    vehicleTypes[index - 1].MaximumSeats = commonMethods.ReadInt("Please enter the new maximum seats : ");
                    while (vehicleTypes[index - 1].MaximumSeats < 0)
                    {
                        vehicleTypes[index - 1].MaximumSeats = commonMethods.ReadInt("Please enter a valid seats greater than 0 : ");
                    }
                    status = vehicleTypeService.UpdateVehicleType(vehicleTypes[index - 1]);
                    if (status == true)
                    {
                        Console.WriteLine("Vehicle Type updated successfully!!");
                        commonMethods.FurtherAction(1);
                    }
                    else
                    {
                        Console.WriteLine("There is an error updating the vehicle type!!");
                        commonMethods.FurtherAction(1);
                    }
                    break;
                case 3: commonMethods.FurtherAction(1);
                    break;
            }
        }

        public bool IsTypeAlreadyExists(string type)
        {
            List<VehicleType> types = vehicleTypeService.GetAllTypes();

            int index = types.IndexOf(types.Find(obj=>obj.Type==type));
            if (index>=0) return true;
            else return false;
        }
    }
}
