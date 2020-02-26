using System;
using System.Collections.Generic;
using System.Linq;
using SGA.Interfaces;
using SGA.Models;

namespace SGA.Lib
{
    public class DataImportAD : IDataImportAD
    {

        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "ImportAD";


        public DataImportAD(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }

        public void ImportADConsultaUsuariosSistema(IQueryable<ApplicationAD> connectionADIQueryable)
        {

            var applicationADList = connectionADIQueryable.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.ConsultaUsuariosSistema).ToList();

            using (var dataImportHelper = new DataImportHelper(_iuw)) {
                int sizeUserDetails = dataImportHelper.GetUserDetailsDictionarySize() + 1;
                int sizeGroupDetails = dataImportHelper.GetGroupDetailsDictionaryySize() + 1;

                HashSet<GroupAccess> groupAccessList = new HashSet<GroupAccess>(new GroupAccess.GroupAccessComparer());
                foreach (var groupAccess in _iuw.GroupAccessRepository.GetList())
                {
                    groupAccessList.Add(groupAccess);
                }

                foreach (var applicationAD in applicationADList)
                {
                    try
                    {
                        List<ApplicationADResult> resultList = GetResultList(applicationAD);

                        foreach (var line in resultList)
                        {
                            UserAccess userAccess = new UserAccess();
                            string username = line.Columns[0];
                            string group = line.Columns[1];

                            sizeUserDetails = dataImportHelper.GetDatabaseUserData(applicationAD.ApplicationId, sizeUserDetails, username, userAccess);
                            sizeGroupDetails = dataImportHelper.GetDatabaseUserAccessGroupData(applicationAD.ApplicationId, sizeGroupDetails, group, userAccess);

                            GroupAccess groupAccess = new GroupAccess();
                            groupAccess.GroupDetailsId = userAccess.GroupDetailsId == 0 ? (int)userAccess.GroupDetails.Id : userAccess.GroupDetailsId;
                            groupAccess.Permission = line.Columns[2] ?? "";

                            if (groupAccessList.Add(groupAccess))
                                _iuw.GroupAccessRepository.Create(groupAccess);

                            _iuw.UserAccessRepository.Create(userAccess);
                        }

                        _iuw.Save();
                        _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Dados da aplicação {applicationAD.Name} para o processo {applicationAD.ApplicationType.Name} foram salvos no banco.");

                        resultList.Clear();
                    }
                    catch (Exception e)
                    {
                        _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar dados da aplicação {applicationAD.Name} para o processo {applicationAD.ApplicationType.Name}. " + e.Message);
                    }
                }
                groupAccessList = null;
                applicationADList = null;
                connectionADIQueryable = null;
            }
        }

        private List<ApplicationADResult> GetResultList(ApplicationAD applicationAD)
        {
            Ldap ldap = _iuw.LdapRepository.Get(x => x.Id == 1);
            if (ldap is null) {
                return null;
            }

            List<ApplicationADResult> resultList = new List<ApplicationADResult>();

            var groupList = new List<string>();
            var groupHashSet = new HashSet<string>();

            using (var adConnection = new ADConnection(ldap)) {
                foreach (var groupFilter in applicationAD.Groups.Split(";"))
                {
                    groupList = adConnection.GetGroupList(groupFilter, startWith: true);
                    foreach (var group in groupList)
                    {
                        groupHashSet.Add(group);
                    }
                }

                resultList.AddRange(adConnection.GetApplicationADResult(groupHashSet));
            }

            groupList = null;
            groupHashSet = null;
            return resultList;
        }


    }
}