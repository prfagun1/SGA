using Novell.Directory.Ldap;
using SGA.Interfaces;
using SGA.Models;
using SGA.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGA.Lib
{
    public class Authentication : IAuthentication
    {
        //private readonly string LogDescription = "Authentication";
        private readonly IUnitOfWork _iuw;
        public Authentication(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }



        public List<Claim> GetClaims(LoginViewModel login)
        {
            Ldap ldap = _iuw.LdapRepository.Get(x => x.Id == 1);
            List<PermissionGroup> groupPermissionList = _iuw.PermissionGroupRepository.GetList().ToList();

            using (ADConnection adAuthentication = new ADConnection(ldap))
            {
                return adAuthentication.GetClaims(login, groupPermissionList);
            }
            
       }


        public bool VerifyAdminUser(LoginViewModel login)
        {
            Parameter parameter = _iuw.ParameterRepository.Get(x => x.Id == 1);

            if (parameter == null) return false;

            if (login.Username == parameter.AdminUser && login.Password == login.Password)
            {
                return true;
            }

            return false;
        }


    }
}
