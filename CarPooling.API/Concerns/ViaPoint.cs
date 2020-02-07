using System;
using System.Collections.Generic;
using System.Text;

namespace CarPooling.API.Concerns
{
    public class ViaPoint
    {
        public string Id { get; set; }

        public string RideId { get; set; }

        public string Name { get; set; }

        public int Distance { get; set; }
    }
}
