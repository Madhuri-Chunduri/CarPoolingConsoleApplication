using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarPooling.API.Concerns
{
    public class Log
    {
        public string Id { get; set; }

        public string Method { get; set; }

        public string Exception { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
