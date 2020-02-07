using CarPooling.Contracts;
using CarPooling.Concerns;
using Dapper;
using DapperExtensions;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CarPooling.Providers
{
    public class UserService : IUserService
    {
        public static List<User> Users = new List<User>();

        public void AddUser(User user)
        {
            Users.Add(user);
        }

        public bool IsValidUser(string email, string password)
        {
            User user = Users.FirstOrDefault(obj => obj.Email == email && obj.Password == password);
            if (user == null) return false;
            else return true;
        }

        public User GetUser(string id)
        {
            return Users.FirstOrDefault(obj => obj.Id == id);
        }

        public User GetUserByMail(string email)
        {
            return Users.FirstOrDefault(obj => obj.Email == email);
        }

        public User UpdateUser(User user)
        {
            User updatedUser = Users.Where(obj => (obj.Email == LoginActions.currentUser.Email))
            .Select(obj => { obj = user; return obj; }).ToList()[0];
            return updatedUser;
        }

        public bool IsExistingUser(string email)
        {
            User user = Users.FirstOrDefault(obj => obj.Email == email);
            if (user == null) return false;
            else return true;
        }
    }
}
