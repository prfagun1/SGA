using SGA.Models;
using SGA.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGA.Interfaces
{
    public interface IAuthentication
    {
        List<Claim> GetClaims(LoginViewModel login);
        bool VerifyAdminUser(LoginViewModel login);
    }
}
