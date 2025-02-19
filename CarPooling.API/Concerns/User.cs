﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.API.Concerns
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public Gender Gender { get; set; }

        public string Password { get; set; }
    }
}
