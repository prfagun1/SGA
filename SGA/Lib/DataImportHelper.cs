using SGA.Interfaces;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Lib
{
    public class DataImportHelper : IDisposable
    {
        private readonly IUnitOfWork _iuw;

        private Dictionary<string, int> userDetailsDictionary;
        private Dictionary<string, int> groupDetailsDictionary;

        public DataImportHelper(IUnitOfWork iuw)
        {
            _iuw = iuw;
            userDetailsDictionary = SetUserDetailsToMemory();
            groupDetailsDictionary = SetGroupDetailsToMemory();
        }

        public int GetUserDetailsDictionarySize() {
            return userDetailsDictionary.Count;
        }

        public int GetGroupDetailsDictionaryySize()
        {
            return groupDetailsDictionary.Count;
        }

        private Dictionary<string, int> SetUserDetailsToMemory()
        {
            return _iuw.UserDetailsRepository.GetList()
                .OrderByDescending(x => x.Id)
                .Select(p => new { p.Username, p.Id })
                .AsEnumerable()
                .Distinct()
                .ToDictionary(x => x.Username, x => x.Id);
        }

        private Dictionary<string, int> SetGroupDetailsToMemory()
        {
            return _iuw.GroupDetailsRepository.GetList()
                .OrderByDescending(x => x.Id)
                .Select(p => new { p.ApplicationId, p.Name, p.Id })
                .AsEnumerable()
                .Distinct()
                .ToDictionary(x => x.ApplicationId.ToString() + x.Name, x => x.Id);
        }

        public int GetDatabaseGroupDetailsData(int applicationId, int sizeGroupDetails, string group, GroupAccess groupAccess)
        {
            int groupDetailsId = groupDetailsDictionary.TryGetValue(applicationId.ToString() + group, out int idGroup) ? idGroup : 0;

            if (idGroup == 0)
            {
                GroupDetails groupDetails = new GroupDetails();
                groupDetails.Name = group;
                groupDetails.ApplicationId = applicationId;
                groupDetails.Id = sizeGroupDetails;

                groupDetailsDictionary.Add(applicationId.ToString() + group, sizeGroupDetails);

                sizeGroupDetails++;

                groupAccess.GroupDetails = groupDetails;
            }
            else
            {
                groupAccess.GroupDetailsId = groupDetailsId;
            }

            return sizeGroupDetails;
        }

        public int GetDatabaseUserAccessGroupData(int applicationId, int sizeGroupDetails, string group, UserAccess userAccess)
        {
            int groupDetailsId = groupDetailsDictionary.TryGetValue(group, out int idGroup) ? idGroup : 0;
            if (idGroup == 0)
            {
                GroupDetails groupDetails = new GroupDetails();
                groupDetails.Name = group;
                groupDetails.ApplicationId = applicationId;
                groupDetails.Id = sizeGroupDetails;

                groupDetailsDictionary.Add(group, sizeGroupDetails);

                sizeGroupDetails++;

                userAccess.GroupDetails = groupDetails;
            }
            else
            {
                userAccess.GroupDetailsId = groupDetailsId;
            }

            return sizeGroupDetails;
        }

        public int GetDatabaseUserData(int applicationId, int sizeUserDetails, string username, UserAccess userAccess)
        {
            int userDetailsId = userDetailsDictionary.TryGetValue(username, out int idUser) ? idUser : 0;

            if (idUser == 0)
            {

                UserDetails userDetails = new UserDetails();
                userDetails.Username = username;
                userAccess.UserDetails = userDetails;
                userDetails.Id = sizeUserDetails;

                userDetailsDictionary.Add(username, sizeUserDetails);

                sizeUserDetails++;
            }
            else
            {
                userAccess.UserDetailsId = userDetailsId;
            }

            userAccess.ApplicationId = applicationId;
            return sizeUserDetails;
        }

        public void Dispose()
        {
            userDetailsDictionary.Clear();
            groupDetailsDictionary.Clear();
        }
    }
}
