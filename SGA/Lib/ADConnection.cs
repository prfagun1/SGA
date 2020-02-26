using Novell.Directory.Ldap;
using SGA.Models;
using SGA.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;

namespace SGA.Lib
{
    public class ADConnection : IDisposable
    {

        private readonly Ldap _ldap = null;
        private PrincipalContext _context;

        public ADConnection(Ldap ldap) {
            _ldap = ldap;
            SetContext();
        }

        public void  CreateUserEmployee(UserCreateEmployee user) {
            String ldapPath = "LDAP://OU=Usuarios Novos,DC=unimed-ners,DC=net";
            string ldapUsername = Cipher.Decrypt(_ldap.BindUser, _ldap.ChangeDate.ToString());
            string ldapPassword = Cipher.Decrypt(_ldap.BindPassword, _ldap.ChangeDate.ToString());

            var name = user.FullName.Split(" ");
            string givenName = name[0];
            string surname = name[name.Length - 1];
            user.Password = Cipher.GetPassword();

            DirectoryEntry conexaoAD = new DirectoryEntry(ldapPath, ldapUsername, ldapPassword);
            DirectoryEntry novoUsuario = conexaoAD.Children.Add("CN=" + user.FullName, "user");

            novoUsuario.Properties["samAccountName"].Value = user.Username;
            novoUsuario.Properties["userPrincipalName"].Value = user.Username + "@xxx";
            novoUsuario.Properties["Company"].Value = "xxx";
            novoUsuario.Properties["countryCode"].Value = 76;
            novoUsuario.Properties["co"].Value = "Brasil";
            novoUsuario.Properties["c"].Value = "BR";
            novoUsuario.Properties["department"].Value = user.Department;
            //novoUsuario.Properties["description"].Value = "";
            novoUsuario.Properties["displayName"].Value = user.FullName;
            novoUsuario.Properties["name"].Value = user.FullName;
            novoUsuario.Properties["employeeID"].Value = user.EmployeeId;
            novoUsuario.Properties["l"].Value = "Caxias do Sul";
            novoUsuario.Properties["physicalDeliveryOfficeName"].Value = user.Department;
            novoUsuario.Properties["st"].Value = "Caxias do Sul";
            novoUsuario.Properties["streetAddress"].Value = "xxx";
            novoUsuario.Properties["telephoneNumber"].Value = "xxx";
            novoUsuario.Properties["wWWHomePage"].Value = "xxx";
            novoUsuario.Properties["givenName"].Value = givenName;
            novoUsuario.Properties["SN"].Value = surname;
            novoUsuario.Properties["mail"].Value = user.Username + "@xxx";
            novoUsuario.Properties["extensionAttribute1"].Value = user.CC;
            novoUsuario.Properties["extensionAttribute3"].Value = user.JobRole;
            novoUsuario.Properties["scriptPath"].Value = "xxx";

            novoUsuario.Properties["title"].Value = user.JobRole;
            novoUsuario.CommitChanges();

            novoUsuario.Invoke("SetPassword", new object[] { user.Password });

            novoUsuario.Properties["userAccountControl"].Value = 512;
            novoUsuario.Properties["pwdlastset"].Value = 0;
            novoUsuario.CommitChanges();


            conexaoAD.Dispose();
            novoUsuario.Dispose();

        }

        public bool GetUserExist(string username) {

            var response = GetUser(username);
            if (response == null)
            {
                return false;
            }
            else {
                return true;
            }
        }

        private void SetContext() {
            string username = Cipher.Decrypt(_ldap.BindUser, _ldap.ChangeDate.ToString());
            string password = Cipher.Decrypt(_ldap.BindPassword, _ldap.ChangeDate.ToString());

            _context = new PrincipalContext(ContextType.Domain, _ldap.Domain, username, password);
        }

        public List<Claim> GetClaims(LoginViewModel login, List<PermissionGroup> permissionGroupList)
        {

            if (login is null || permissionGroupList is null) return null;

            Boolean userVerifyPassword = ValidateCredentials(login.Username, login.Password);

            //userVerifyPassword = true;
            if (userVerifyPassword)
            {
                return VerifyGroupPermission(login.Username, permissionGroupList);
            }

            return null;
        }

        public bool ValidateCredentials(string username, string password)
        {
            return _context.ValidateCredentials(username, password);
        }


        private List<Claim> VerifyGroupPermission(String username, List<PermissionGroup> permissionGroupList)
        {
            List<Claim> claims = new List<Claim>();

            try
            {
                foreach (PermissionGroup group in permissionGroupList)
                {
                    var user = GetGroupUserList(group.Name).Where(x => x.SamAccountName == username).FirstOrDefault();
                    if (user is null) continue;

                    claims.AddRange(Lib.AuthenticationHelper.GetClaimType(group.AccessType));
                }
            }
            catch { }

            return claims;
        }

        public List<Principal> GetGroupUserList(String group, bool recursive = true)
        {
            List<Principal> groupList = new List<Principal>();
            using (var gpSeach = new PrincipalSearcher(new GroupPrincipal(_context)))
            {
                foreach (GroupPrincipal gp in gpSeach.FindAll().Where(gp => gp.Name == group))
                {

                    foreach (Principal member in gp.Members)
                    {
                        if (member.StructuralObjectClass == "group" && recursive)
                        {
                            groupList.AddRange(GetGroupUserListRecursive(gp));
                        }
                        else
                        {
                            groupList.Add(member);
                        }
                    }

                    /*
                    foreach (Principal member in gp.Members)
                    {
                        if (member.StructuralObjectClass == "group" && recursive)
                        {
                            groupList.AddRange(GetGroupUserList(member.Name));
                        }
                        else
                        {
                            groupList.Add(member);
                        }
                    }
                    */
                }
            }
            return groupList;
        }

        public List<Principal> GetGroupUserListRecursive(GroupPrincipal gp) {
            
            List<Principal> groupList = new List<Principal>();

            foreach (Principal member in gp.Members)
            {
                if (member.StructuralObjectClass == "group")
                {
                    groupList.AddRange(GetGroupUserListRecursive(member as GroupPrincipal));
                }
                else
                {
                    groupList.Add(member);
                }
            }
            return groupList;
        }



        public List<ApplicationADResult> GetApplicationADResult(HashSet<string> groupList) {

            List<ApplicationADResult> resultList = new List<ApplicationADResult>();

            foreach (var group in groupList) {
                var userList = GetGroupUserList(group, recursive: false);
                var groupPrincipal = GetGroup(group);

                DirectoryEntry de = groupPrincipal.GetUnderlyingObject() as DirectoryEntry;
                string groupInfo = "";
                if (de.Properties["info"].Value != null)
                {
                    groupInfo = de.Properties["info"].Value.ToString();
                }

                foreach (var user in userList) {
                    if (user.StructuralObjectClass == "group")
                        continue;

                    var result = new ApplicationADResult();

                    result.AddColumns(user.SamAccountName);
                    result.AddColumns(group);
                    result.AddColumns(groupInfo);

                    resultList.Add(result);
                }
                userList = null;
                groupPrincipal = null;
            }

            return resultList;
        }


        internal void EnableUser(string username)
        {
            UserPrincipal oUserPrincipal = GetUser(username);
            oUserPrincipal.Enabled = true;
            oUserPrincipal.Save();
        }


        public UserPrincipal GetUser(string sUserName)
        {
            return  UserPrincipal.FindByIdentity(_context, sUserName);
        }

        public List<string> GetGroupList(String group, bool startWith = false)
        {
            List<string> groupList = new List<string>();
            using (var gpSeach = new PrincipalSearcher(new GroupPrincipal(_context)))
            {
                foreach (GroupPrincipal gp in gpSeach.FindAll().Where(gp => gp.Name == group || (startWith && gp.Name.ToUpper().StartsWith(group.ToUpper()) )))
                {
                    groupList.AddRange(GetGroupListRecursive(gp));
                }

            }
            return groupList;
        }

        public List<string> GetGroupListRecursive(GroupPrincipal gp) {
            List<string> groupList = new List<string>();

            foreach (Principal member in gp.Members)
            {
                if (member.StructuralObjectClass == "group")
                {
                    groupList.AddRange(GetGroupListRecursive(member as GroupPrincipal));
                }
            }
            groupList.Add(gp.Name);

            return groupList;
        }


        public GroupPrincipal GetGroup(string groupName)
        {
            return GroupPrincipal.FindByIdentity(_context, groupName);
        }

        internal void DisableUserTemporary(string username)
        {
            UserPrincipal oUserPrincipal = GetUser(username);
            oUserPrincipal.Enabled = false;
            oUserPrincipal.Save();
        }

        public void DisableUser(string username)
        {
            UserPrincipal oUserPrincipal = GetUser(username);
            oUserPrincipal.Enabled = false;
            oUserPrincipal.Save();

            MoveDisableUser(oUserPrincipal);

            oUserPrincipal.Dispose();
        }

        internal void MoveDisableUser(UserPrincipal oUserPrincipal) {
            String destiny = "OU=Usuarios Desligados,DC=xxx,DC=net";
            string username = Cipher.Decrypt(_ldap.BindUser, _ldap.ChangeDate.ToString());
            string password = Cipher.Decrypt(_ldap.BindPassword, _ldap.ChangeDate.ToString());

            if (oUserPrincipal.DistinguishedName.Contains("Usuarios Desligados")) {
                return;
            }

            DirectoryEntry ouOrigin = new DirectoryEntry("LDAP://" + oUserPrincipal.DistinguishedName, username, password);
            DirectoryEntry ouDestination = new DirectoryEntry("LDAP://" + destiny, username, password);

            String nomeUsuario = ouOrigin.Name;
            int contador = 1;
            bool deveAlterar = false;

//O AD não permite salvar duas pessoas com o mesno nome na mesma OU
//O procedimento abaixo verifica isso, caso já exista o nome na OU adiciona ' - 1,2,...' no fim
            while (SearchDisabledUser(nomeUsuario, destiny))
            {
                nomeUsuario = nomeUsuario + " - " + contador;
                deveAlterar = true;
            }

            if (deveAlterar)
            {
                this.ChangeUserDisplayName(oUserPrincipal.DistinguishedName, nomeUsuario, ouDestination);
                ouOrigin.Close();
                ouDestination.Close();
            }
            else
            {
                ouOrigin.MoveTo(ouDestination);
                ouOrigin.Close();
                ouDestination.Close();
            }
        }

        public bool ChangeUserDisplayName(String dnAtual, String dnNovo, DirectoryEntry destino)
        {
            String ldapPath = "LDAP://" + dnAtual;
            string ldapUsername = Cipher.Decrypt(_ldap.BindUser, _ldap.ChangeDate.ToString());
            string ldapPassword = Cipher.Decrypt(_ldap.BindPassword, _ldap.ChangeDate.ToString());

            DirectoryEntry child = new DirectoryEntry(ldapPath, ldapUsername, ldapPassword);
            child.Rename(dnNovo);
            child.MoveTo(destino);
            child.Close();
            child.Dispose();
            return true;

        }

        public bool SearchDisabledUser(string username, string destiny)
        {
            username = username.Replace("CN=", "");

            UserPrincipal user = new UserPrincipal(_context);
            user.DisplayName = username;

            PrincipalSearcher srch = new PrincipalSearcher(user);

            foreach (var encontrado in srch.FindAll())
            {
                if (encontrado.DistinguishedName.Contains(destiny))
                {
                    return true;
                }
            }
            return false;
        }


        public void Dispose()
        {
            _context.Dispose();
        }

        internal string ChangePassword(string username)
        {
            string password = Lib.Cipher.GetPassword();
            UserPrincipal userPrincipal = GetUser(username);
            userPrincipal.SetPassword(password);
            userPrincipal.ExpirePasswordNow();
            return password;
        }
    }
}
