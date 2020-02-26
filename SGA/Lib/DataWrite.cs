using Microsoft.EntityFrameworkCore;
using SGA.Interfaces;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SGA.Lib
{
    public class DataWrite : IDataWrite
    {

        private readonly IUnitOfWork _iuw;
        private readonly IDataImport _dataImport;
        private readonly string LogDescription = "DataWrite";
        private IDataImportRest _dataImportRest;

        public DataWrite(IUnitOfWork iuw, IDataImport dataImport, IDataImportRest dataImportRest)
        {
            _iuw = iuw;
            _dataImport = dataImport;
            _dataImportRest = dataImportRest;
        }

        /// <summary>
        /// Caso não seja imformado parâmetro, importa todos os usuários para após desativar
        /// </summary>
        /// <param name="userHR">Usuário que será desativado</param>
        public bool EnableDisableUsers(string username = null, EnumSGA.UserHRStatusHR UserHRStatusHR = EnumSGA.UserHRStatusHR.Desligado)
        {
            bool status = true;
            IList<UserHR> userHRList;

            //Caso o usuário seja null significa que é para executar a rotina de desativação de todos os usuários da lista
            if (username is null)
            {
                status = _dataImport.ImportUserHRList();
                userHRList = _iuw.UserHRRepository.GetList(new List<Expression<Func<UserHR, bool>>>
                {
                    x => x.StatusLocal == EnumSGA.UserHRStatusLocal.Pendente

                }).ToList();
            }
            else
            {
                userHRList = new List<UserHR>();
                userHRList.Add(GetUserHRFromUsername(username, UserHRStatusHR));
            }

            EnableDisableUsersInternal(userHRList.ToList());

            return status;
        }

        private void EnableDisableUsersInternal(List<UserHR> userHRList)
        {
            Ldap ldap = _iuw.LdapRepository.Get(x => x.Id == 1);

            var applicationRestList = _iuw.ApplicationRestRepository.GetList(new List<Expression<Func<ApplicationRest, bool>>>
            {
                x => (x.ApplicationTypeId == (int)EnumSGA.ConnectionType.AtivaUsuariosSistema ||
                x.ApplicationTypeId == (int)EnumSGA.ConnectionType.DesativaUsuariosSistema) &&
                x.Enable == EnumSGA.Status.Enabled
            }).AsNoTracking();


            var applicationSQLList = _iuw.ApplicationSQLRepository.GetList(new List<Expression<Func<ApplicationSQL, bool>>>
            {
                x => (x.ApplicationTypeId == (int)EnumSGA.ConnectionType.AtivaUsuariosSistema ||
                x.ApplicationTypeId == (int)EnumSGA.ConnectionType.DesativaUsuariosSistema) &&
                x.Enable == EnumSGA.Status.Enabled
            }, x => x.DatabaseSGA).AsNoTracking();


            string[] exceptionHR = { };

            var exclusionList = _iuw.ParameterRepository.Get(x => x.Id == 1);

            if (exclusionList.ExclusionListAfastamento != null)
            {
                exceptionHR = exclusionList.ExclusionListAfastamento.Split(',');
            }


            foreach (var userHR in userHRList)
            {
                try
                {
                    switch (userHR.StatusRH)
                    {

                        case EnumSGA.UserHRStatusHR.Desligado:
                            if (userHR.MetadadosDate <= DateTime.Now)
                            {
                                EnableDisableUserApplication(
                                userHR,
                                applicationSQLList.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.DesativaUsuariosSistema),
                                applicationRestList.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.DesativaUsuariosSistema)
                            );

                                using (var adConnection = new ADConnection(ldap))
                                {
                                    adConnection.DisableUser(userHR.Username);
                                }
                                userHR.StatusLocal = EnumSGA.UserHRStatusLocal.OK;
                            }
                            break;


                        case EnumSGA.UserHRStatusHR.Afastado:
                            if (userHR.MetadadosDate <= DateTime.Now && Array.FindIndex(exceptionHR, s => s.Trim().Equals(userHR.Username)) < 0)
                            {
                                EnableDisableUserApplication(
                                    userHR,
                                    applicationSQLList.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.DesativaUsuariosSistema),
                                    applicationRestList.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.DesativaUsuariosSistema)
                                );

                                using (var adConnection = new ADConnection(ldap))
                                {
                                    adConnection.DisableUserTemporary(userHR.Username);
                                }

                                //Testa se a data de retorno não foi enviada pelo metadados, quando é 31/12/9999
                                //Quando o funcionário retornar será enviado um novo registro
                                if (userHR.MetadadosDateReturn > DateTime.Now.AddYears(100)) {
                                    userHR.StatusLocal = EnumSGA.UserHRStatusLocal.OK;
                                }
                                userHR.StatusLocal = EnumSGA.UserHRStatusLocal.Afastado;
                            }

                            break;


                        case EnumSGA.UserHRStatusHR.Voltando:
                            if (userHR.StatusLocal == EnumSGA.UserHRStatusLocal.Afastado && userHR.MetadadosDateReturn <= DateTime.Now && Array.FindIndex(exceptionHR, s => s.Trim().Equals(userHR.Username)) < 0)
                            {
                                EnableDisableUserApplication(
                                userHR,
                                applicationSQLList.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.AtivaUsuariosSistema),
                                applicationRestList.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.AtivaUsuariosSistema)
                            );

                                using (var adConnection = new ADConnection(ldap))
                                {
                                    adConnection.EnableUser(userHR.Username);
                                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Desativado usuário {userHR.Username} no Active Directory.");
                                }
                                userHR.StatusLocal = EnumSGA.UserHRStatusLocal.OK;
                            }
                            break;
                    }

                    userHR.ChangeDate = DateTime.Now;

                    //Deve ser melhorado - seta que desativou no AD
                    userHR.UserHRApplication.Add(new UserHRApplication
                    {
                        ApplicationId = 5,
                        UserHRId = userHR.Id
                    });
                    
                    
                    _iuw.UserHRRepository.Update(userHR);
                    _iuw.Save();

                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Usuário {userHR.Username} desativado.");
                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao desativar usuário {userHR.Username}, mensagem {e.ToString()}.");
                }


            }

        }

        private void EnableDisableUserApplication(UserHR userHR, IQueryable<ApplicationSQL> applicationSQLList, IQueryable<ApplicationRest> applicationRestList)
        {
            var databaseConnection = new DatabaseConnection(_iuw);
            Dictionary<int, bool> applicationValues = new Dictionary<int, bool>();

            foreach (var applicationSQL in applicationSQLList)
            {
                try
                {
                    applicationSQL.SQL = applicationSQL.SQL.Replace("UsuarioDesativadoSistema", userHR.Username);
                    int status = databaseConnection.SetDatabaseValues(applicationSQL);

                    if (status > 0)
                    {
                        //Verifica se existe mais de uma entrada para a mesma aplicação
                        if (!applicationValues.TryGetValue(applicationSQL.ApplicationId, out bool applicationStatus))
                        {
                            applicationValues.Add(applicationSQL.ApplicationId, true);
                            userHR.UserHRApplication.Add(new UserHRApplication
                            {
                                ApplicationId = applicationSQL.ApplicationId,
                                UserHRId = userHR.Id
                            });
                        }

                        _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Desativado usuário {userHR.Username} no sistema {applicationSQL.Name}.");
                    }
                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao desativar usuário senha do usuário {userHR.Username} no procedimento {applicationSQL.Name}, mensagem {e.ToString()}.");
                }
            }


            foreach (var applicationRest in applicationRestList)
            {
                try
                {
                    applicationRest.Json = applicationRest.Json.Replace("UsuarioDesativadoSistema", userHR.Username);
                    int status = _dataImportRest.SetRestValue(applicationRest, out string json);

                    //Verifica se existe mais de uma entrada para a mesma aplicação
                    if (!applicationValues.TryGetValue(applicationRest.ApplicationId, out bool applicationStatus))
                    {
                        applicationValues.Add(applicationRest.ApplicationId, true);

                        userHR.UserHRApplication.Add(new UserHRApplication
                        {
                            ApplicationId = applicationRest.ApplicationId,
                            UserHRId = userHR.Id
                        });

                    }


                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Desativado usuário {userHR.Username} no sistema {applicationRest.Name}.");
                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao desativar usuário senha do usuário {userHR.Username} no procedimento {applicationRest.Name}, mensagem {e.ToString()}.");
                }
            }

        }

        private UserHR GetUserHRFromUsername(string username, EnumSGA.UserHRStatusHR UserHRStatusHR)
        {
            var userDetails = _iuw.UserDetailsRepository.Get(x => x.Username == username);
            UserHR userHR = new UserHR();
            userHR.Username = userDetails.Username;
            userHR.JobRole = userDetails.JobRole;
            userHR.CC = userDetails.CC;
            userHR.Department = userDetails.Department;
            userHR.FullName = userDetails.FullName;
            userHR.StatusRH = UserHRStatusHR;
            userHR.StatusLocal = EnumSGA.UserHRStatusLocal.Pendente;
            return userHR;
        }

        public string ChangePassword(string username)
        {
            Ldap ldap = _iuw.LdapRepository.Get(x => x.Id == 1);
            using (var adConnection = new ADConnection(ldap))
            {
                string password = adConnection.ChangePassword(username);
                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Alterada senha do usuário {username}.");
                return password;
            }

        }
    }
}