using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Interfaces
{
    public interface IDataWrite
    {
        bool EnableDisableUsers(string username = null, EnumSGA.UserHRStatusHR UserHRStatusHR = EnumSGA.UserHRStatusHR.Desligado);
        string ChangePassword(string username);
    }
}
