using CarPooling.API.Concerns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarPooling.API.Contracts
{
    public interface IStatusService
    {
        Status GetStatus(string type,string value);

        bool AddStatus(Status status);

        bool UpdateStatus(Status status);
    }
}
