using SGA.Interfaces;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Lib
{
    public class DataImportSQL : IDataImportSQL
    {
        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "ImportSQL";

        public DataImportSQL(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }

        private void SQLSaveDatabaseUserDetails(List<ApplicationSQLResult> resultList)
        {
            foreach (var line in resultList)
            {
                int cc = 0;
                string username = line.Columns[0];

                var userDetailsDatabase = _iuw.UserDetailsRepository.Get(x => x.Username == username);
                if (userDetailsDatabase == null)
                {
                    UserDetails userDetails = new UserDetails();
                    userDetails.Username = username;
                    userDetails.FullName = line.Columns[1];
                    userDetails.JobRole = line.Columns[2];
                    userDetails.Department = line.Columns[3];
                    Int32.TryParse(line.Columns[4], out cc);
                    userDetails.CC = cc;
                    _iuw.UserDetailsRepository.Create(userDetails);
                }
                else
                {
                    userDetailsDatabase.Username = username;
                    userDetailsDatabase.FullName = line.Columns[1];
                    userDetailsDatabase.JobRole = line.Columns[2];
                    userDetailsDatabase.Department = line.Columns[3];
                    Int32.TryParse(line.Columns[4], out cc);
                    userDetailsDatabase.CC = cc;
                    _iuw.UserDetailsRepository.Update(userDetailsDatabase);
                }
            }

            _iuw.Save();
        }

        public void ImportSQLConsultaGruposPermissoes(IQueryable<ApplicationSQL> connectionSQLIQueryable)
        {
            var applicationSQLList = connectionSQLIQueryable.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.ConsultaGruposPermissoes).ToList();
            var databaseConnection = new DatabaseConnection(_iuw);

            try
            {
                using (var dataImportHelper = new DataImportHelper(_iuw))
                {

                    int sizeGroupDetails = dataImportHelper.GetGroupDetailsDictionaryySize() + 1;

                    foreach (var applicationSQL in applicationSQLList)
                    {
                        try
                        {
                            List<ApplicationSQLResult> resultList = databaseConnection.GetDatabaseValues(applicationSQL);

                            foreach (var line in resultList)
                            {
                                GroupAccess groupAccess = new GroupAccess();

                                string group = line.Columns[0];
                                string permission = line.Columns[1];

                                sizeGroupDetails = dataImportHelper.GetDatabaseGroupDetailsData(applicationSQL.ApplicationId, sizeGroupDetails, group, groupAccess);

                                groupAccess.Permission = permission;
                                _iuw.GroupAccessRepository.Create(groupAccess);
                            }

                            resultList = null;
                            _iuw.Save();
                            _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Dados da aplicação {applicationSQL.Name} para o processo {applicationSQL.ApplicationType.Name} foram salvos no banco.");
                        }
                        catch (Exception e)
                        {
                            _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar dados da aplicação {applicationSQL.Name} para o processo {applicationSQL.ApplicationType.Name}. {e.Message}. Exceção interna {e.InnerException.ToString()}");
                        }
                    }

                }
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar dados da aplicação para o processo ImportSQLConsultaGruposPermissoes. " + e.Message);
            }

        }

        public void ImportSQLListagemDetalhesUsuarios(IQueryable<ApplicationSQL> connectionSQLIQueryable)
        {
            var applicationSQLList = connectionSQLIQueryable.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.ListagemDetalhesUsuarios).ToList();
            var databaseConnection = new DatabaseConnection(_iuw);

            foreach (var applicationSQL in applicationSQLList)
            {
                try
                {
                    List<ApplicationSQLResult> result = databaseConnection.GetDatabaseValues(applicationSQL);
                    SQLSaveDatabaseUserDetails(result);
                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Dados da aplicação {applicationSQL.Name} para o processo {applicationSQL.ApplicationType.Name} foram salvos no banco.");
                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Erro ao salvar dados da aplicação {applicationSQL.Name} para o processo {applicationSQL.ApplicationType.Name}. {e.Message}. Exceção interna {e.InnerException.ToString()}");
                }
            }
        }

        public void ImportSQLConsultaUsuariosSistema(IQueryable<ApplicationSQL> connectionSQLIQueryable)
        {
            var applicationSQLList = connectionSQLIQueryable.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.ConsultaUsuariosSistema).ToList();
            var databaseConnection = new DatabaseConnection(_iuw);

            using (var dataImportHelper = new DataImportHelper(_iuw))
            {
                int sizeUserDetails = dataImportHelper.GetUserDetailsDictionarySize() + 1;
                int sizeGroupDetails = dataImportHelper.GetGroupDetailsDictionaryySize() + 1;

                foreach (var applicationSQL in applicationSQLList)
                {
                    try
                    {
                        _iuw.Save();
                        List<ApplicationSQLResult> resultList = databaseConnection.GetDatabaseValues(applicationSQL);

                        foreach (var line in resultList)
                        {
                            UserAccess userAccess = new UserAccess();
                            string username = line.Columns[0];
                            string group = line.Columns[1];

                            sizeUserDetails = dataImportHelper.GetDatabaseUserData(applicationSQL.ApplicationId, sizeUserDetails, username, userAccess);
                            sizeGroupDetails = dataImportHelper.GetDatabaseUserAccessGroupData(applicationSQL.ApplicationId, sizeGroupDetails, group, userAccess);

                            _iuw.UserAccessRepository.Create(userAccess);

                        }

                        resultList = null;
                        _iuw.Save();
                        _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Dados da aplicação {applicationSQL.Name} para o processo {applicationSQL.ApplicationType.Name} foram salvos no banco.");
                    }
                    catch (Exception e)
                    {
                        _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar dados da aplicação {applicationSQL.Name} para o processo {applicationSQL.ApplicationType.Name}. " + e.Message + e.InnerException ?? "");
                    }
                }
            }

        }

    }
}
