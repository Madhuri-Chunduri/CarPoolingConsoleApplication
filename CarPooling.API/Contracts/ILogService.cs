using CarPooling.API.Concerns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarPooling.API.Contracts
{
    public interface ILogService
    {
        bool LogException(string Exception, string MethodName);

        List<Log> GetAllLogItems();
    }
}
