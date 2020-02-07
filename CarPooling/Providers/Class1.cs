using CarPooling.Concerns;
using CarPooling.Contracts;
using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CarPooling.Services
{
    class Class1
    {
        string connectionString = "";
        public void AddUser(User user)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.Insert(user);
            }
        }

        public bool IsValidUser(string email, string password)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                User user = con.Query<User>("select * from [User] where Email=@Email and Password=@Password", new { Email = email, Password = password }).FirstOrDefault();
                //con.Query<Department>("insert into Department(Name,HOD) values(@Name,@HOD)",new { department.Name, department.HOD });
                if (user == null) return false;
                else return true;
            }
            //User user = Users.FirstOrDefault(obj=> obj.Email==email && obj.Password==password);
        }

        public User GetUser(string id)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                return con.Query<User>("select * from [User] where Id=@Id", new { Id = id }).FirstOrDefault();
            }
        }

        public User GetUserByMail(string email)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                return con.Query<User>("select * from [User] where Email=@Email", new { Email = email }).FirstOrDefault();
            }
        }

        public User UpdateUser(User user)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.Update(user);
                return con.Get<User>(user);
            }
        }

        public bool IsExistingUser(string email)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                User user = con.Query<User>("select * from [User] where Email=@Email", new { Email = email }).FirstOrDefault();
                if (user == null) return false;
                else return true;
            }
        }
    }
}
