using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace CarPooling
{
    public class CommonMethods
    {
        public string ReadString(string label)
        {
            Console.Write(label);
            string value = Console.ReadLine();
            while(value=="")
            {
                Console.Write("Please enter a valid value : ");
                value = Console.ReadLine();
            }
            return value;
        }

        public int ReadInt(string message)
        {
            Console.Write(message);
            int value = 0;
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Please enter a valid value : ");
            }
            return value;
        }

        public double ReadDouble(string message)
        {
            Console.Write(message);
            double value = 0;
            while (!double.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Please enter a valid value : ");
            }
            while(value<=0)
            {
                ReadDouble(message);
            }
            return value;
        }

        public bool IsValidEmail(string mail)
        {
            try
            {
                MailAddress m = new MailAddress(mail);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool IsValidPhoneNumber(string number)
        {
            if (number.Length < 10 || number.Length > 12) return false;
            return Regex.Match(number, @"^[0-9]\d*$").Success;
        }

        public int IsValidChoice(int choice)
        {
            while (choice > 3 || choice < 1)
            {
                choice = ReadInt("Please enter a choice between 1 and 2 : ");
            }
            return choice;
        }

        public void FurtherAction(int choice)
        {
            UserActions UserActions = new UserActions();
            if (choice == 1)
            {
                System.Threading.Thread.Sleep(1500);
                UserActions.DisplayMethods();
            }
            else
            {
                LoginActions LoginActions = new LoginActions();
                LoginActions.LogOut();
            }
        }
    }
}
