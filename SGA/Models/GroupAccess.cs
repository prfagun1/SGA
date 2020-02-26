using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;


namespace SGA.Models
{
    public class GroupAccess
    {
        public GroupAccess()
        {
            //this.GroupDetails = new List<GroupDetails>();
        }

        public int GroupDetailsId { get; set; }

        [DisplayName("Permission")]
        public string Permission { get; set; }

        public virtual GroupDetails GroupDetails { get; set; }


        internal class GroupAccessComparer : IEqualityComparer<GroupAccess>
        {
            public bool Equals(GroupAccess x, GroupAccess y)
            {
                if (x == null || y == null)
                    return false;

                if (x == y)
                    return true;

                if (x.GroupDetailsId == y.GroupDetailsId && x.Permission == y.Permission)
                    return true;

                return false;
            }

            public int GetHashCode(GroupAccess obj)
            {
                return obj.Permission.GetHashCode() + obj.GroupDetailsId.GetHashCode();
            }
        }
    }
}
