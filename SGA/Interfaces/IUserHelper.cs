using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Interfaces
{
    public interface IUserHelper
    {
        void CreateEmployeeUser(List<UserCreateEmployee> userList);
    }
}
