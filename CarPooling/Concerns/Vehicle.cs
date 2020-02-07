using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.Concerns
{
    public class Vehicle
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Model { get; set; }

        public string Number { get; set; }

        public bool IsActive { get; set; }
    }
}
