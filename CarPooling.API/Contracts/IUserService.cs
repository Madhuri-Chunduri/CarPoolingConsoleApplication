using CarPooling.API.Concerns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.API.Contracts
{
    public interface IUserService
    {
        string AddUser(User user);

        User GetUser(string id);

        bool IsValidUser(string email, string password);

        bool IsExistingUser(string email);

        User GetUserByMail(string email);

        bool UpdateUser(User user);
    }
}
