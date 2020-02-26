using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SGA.Interfaces;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SGA.Lib
{

    public class UserHelper : IUserHelper
    {
        private readonly IUnitOfWork _iuw;
        private readonly IDataImportRest _dataImportRest;
        private readonly string LogDescription = "UserHelper";

        public UserHelper(IUnitOfWork iuw, IDataImportRest dataImportRest)
        {
            _iuw = iuw;
            _dataImportRest = dataImportRest;

        }


        public void CreateEmployeeUser(List<UserCreateEmployee> userList) {

            var ldap = _iuw.LdapRepository.Get(x => x.Id == 1);

            foreach (var user in userList)
            {

                string login = GetLogin(user.FullName);
                bool userExist = GetUserExist(login);
                if (userList.Where(x => x.Username == login).FirstOrDefault() != null) userExist = true;

                int count = 1;

                while (userExist){
                    string loginTest = login;

                    if(count >= 10)
                    {
                        if (loginTest.Length >= 7)
                        {
                            loginTest = loginTest.Substring(0, 5) + count.ToString();
                        }
                        else
                        {
                            loginTest = loginTest + count.ToString();
                        }
                    }

                    if (loginTest.Length >= 8)
                    {
                        loginTest = loginTest.Substring(0, 6) + count.ToString();
                    }
                    else {
                        loginTest = loginTest + count.ToString();
                    }

                    count++;
                    userExist = GetUserExist(loginTest);
                    if (userList.Where(x => x.Username == loginTest).FirstOrDefault() != null) userExist = true;

                    if (!userExist) login = loginTest;

                }
                user.Username = login;

                
                using (var ad = new ADConnection(ldap))
                {
                    try
                    {
                        ad.CreateUserEmployee(user);
                        _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Active Directory - Usuário {user.Username} criado para o funcionário {user.FullName}.");

                        CreateUserApplication(user.Username, user.FullName);
                        user.Status = EnumSGA.UserCreateEmployeeStatus.OK;

                        _iuw.UserCreateEmployeeRepository.Update(user);
                        _iuw.Save();
                    }
                    catch (Exception e)
                    {
                        _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Active Directory - Erro ao criar usuário {user.Username} para o funcionário {user.FullName}. " + e.Message);
                    }

                }

            }
          
        }

        public void CreateUserApplication(string username, string fullName) {

            var applicationRestList = _iuw.ApplicationRestRepository.GetList(new List<Expression<Func<ApplicationRest, bool>>>
            {
                x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.CriarUsuarios
            }, x => x.Application).AsNoTracking();


            var applicationSQLList = _iuw.ApplicationSQLRepository.GetList(new List<Expression<Func<ApplicationSQL, bool>>>
            {
                x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.CriarUsuarios
            }, x => x.DatabaseSGA, x => x.Application).AsNoTracking();



            var databaseConnection = new DatabaseConnection(_iuw);

            foreach (var applicationSQL in applicationSQLList)
            {
                try
                {
                    applicationSQL.SQL = applicationSQL.SQL.Replace("UsuarioCriadoUsuario", username);
                    applicationSQL.SQL = applicationSQL.SQL.Replace("UsuarioCriadoNomeCompleto", fullName);
                    int retorno = databaseConnection.SetDatabaseValues(applicationSQL);

                    if (retorno > 0) {
                        _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"{applicationSQL.Application.Name} - Usuário {username} criado para o funcionário {fullName}.");
                    }
                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"{applicationSQL.Application.Name} - Erro ao criar usuário {username} para o funcionário {fullName}. " + e.Message);
                }

            }

            foreach (var applicationRest in applicationRestList)
            {
                try
                {
                    string json;
                    applicationRest.Json = applicationRest.Json.Replace("UsuarioCriadoUsuario", username);
                    applicationRest.Json = applicationRest.Json.Replace("UsuarioCriadoNomeCompleto", fullName);


                    int retorno = _dataImportRest.SetRestValue(applicationRest, out json);
                    if (retorno >= 0)
                    {
                        _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"{applicationRest.Application.Name} - Usuário {username} criado para o funcionário {fullName}. {json}");
                    }
                    else {
                        _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"{applicationRest.Application.Name} - Erro ao criar usuário {username} para o funcionário {fullName}. {json}");
                    }

                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"{applicationRest.Application.Name} - Erro ao criar usuário {username} para o funcionário {fullName}. " + e.Message);
                }
            }
        }


        public string GetLogin(string FullName) {
            string username = "";
            int lenght;

            FullName = RemoveAccents(FullName);

            var name = FullName.Split(" ");
            username += name[0].Substring(0, 1);

            if (name[name.Length - 1].Length >= 7)
            {
                lenght = 7;
            }
            else {
                lenght = name[name.Length - 1].Length;
            }

            username += name[name.Length - 1].Substring(0, lenght);

            return username.ToLower();
        }


        public static string RemoveAccents(string texto)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
            {
                texto = texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
            }
            return texto;
        }


        public bool GetUserExist(string username) {

            var ldap = _iuw.LdapRepository.Get(x => x.Id == 1);

            bool userMirth = GetUserExistMirth(username);
            bool userAD = false;

            using (var ad = new ADConnection(ldap))
            {
                userAD = ad.GetUserExist(username);
            }

            if (userAD || userMirth) {
                return true;
            }

            return false;
        }

        public bool GetUserExistMirth(String username) {
            var parameter = _iuw.ParameterRepository.Get(x => x.Id == 1);

            ApplicationRest applicationRest = new ApplicationRest();
            applicationRest.URL = parameter.ValidaUsuarioURL;
            applicationRest.Username = parameter.ValidaUsuarioUsername;
            applicationRest.Password = parameter.ValidaUsuarioPassword;
            applicationRest.Json = parameter.ValidaUsuarioJson.Replace("UsuarioDoSistema", username);
            applicationRest.MD5 = true;
            applicationRest.RestType = EnumSGA.RestType.POST;
            applicationRest.ChangeDate = parameter.ChangeDate;

            var response = _dataImportRest.GetRestValue(applicationRest);

            dynamic json = JsonConvert.DeserializeObject(response);

            return json.status;
        }

    }


}
