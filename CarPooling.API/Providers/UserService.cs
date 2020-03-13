using CarPooling.API.Concerns;
using CarPooling.API.Contracts;
using Dapper;
using DapperExtensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace CarPooling.API.Providers
{
    public class UserService : IUserService
    {
        public IConfiguration configuration;
        public string connectionString;

        public UserService(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetSection("Connectionstring").Value;
        }

        //public bool AddNewUser(UserDetails user)
       // {
       //     user.Id = Guid.NewGuid().ToString();
       //     string query = "insert into [User](Id,Name,Email,Password) values(@Id,@Name,@Email,@Password)";
       //     DynamicParameters parameters = new DynamicParameters();
       //     parameters.Add("Id", user.Id);
      //      parameters.Add("Name", user.Name);
       //     parameters.Add("Email", user.Email);
        //    parameters.Add("Password", user.Password);
        //    ExtensionObject extensionObject = new ExtensionObject()
        //    {
        //        Query = query,
        //        ConnectionString = connectionString
        //    };
        //    return extensionObject.AddOrUpdateItem<UserDetails>(parameters);
       // }

        public string AddUser(User user)
        {
            user.Id = Guid.NewGuid().ToString();
            string query = "insert into [User](Id,Name,Email,PhoneNumber,Address,Gender,Password) values(@Id,@Name,@Email,@PhoneNumber,@Address,@Gender,@Password)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", user.Id);
            parameters.Add("Name", user.Name);
            parameters.Add("Email", user.Email);
            parameters.Add("PhoneNumber", user.PhoneNumber);
            parameters.Add("Address", user.Address);
            parameters.Add("Gender", user.Gender);
            parameters.Add("Password", user.Password);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            if (extensionObject.AddOrUpdateItem<User>(parameters))
                return user.Id;
            else return null;
        }

        public User GetUser(string id)
        {
            string query = "select * from [User] where Id='" + id + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetItem<User>();
        }

        public User GetUserByMail(string email)
        {
            string query = "select * from [User] where Email='" + email +"'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.GetItem<User>();
        }

        public bool IsExistingUser(string email)
        {
            string query = "select * from [User] where Email='" + email + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            User user = extensionObject.GetItem<User>();
            if (user == null) return false;
            else return true;
        }

        public bool IsValidUser(string email, string password)
        {
            string query = "select * from [User] where Email='" + email + "' and Password='" + password + "'";
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            User user = extensionObject.GetItem<User>();
            if (user == null) return false;
            return true;
        }

        public bool UpdateUser(User user)
        {
            string query = "update [User] set Name=@Name,Email=@Email,PhoneNumber=@PhoneNumber,Address=@Address,Password=@Password where Id=@Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", user.Id);
            parameters.Add("Name", user.Name);
            parameters.Add("Email", user.Email);
            parameters.Add("PhoneNumber", user.PhoneNumber);
            parameters.Add("Address", user.Address);
            parameters.Add("Password", user.Password);
            ExtensionObject extensionObject = new ExtensionObject()
            {
                Query = query,
                ConnectionString = connectionString
            };
            return extensionObject.AddOrUpdateItem<User>(parameters);
        }
    }
}