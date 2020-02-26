using System.Collections.Generic;
using System.ComponentModel;

namespace SGA.Models
{
    public class GroupDetails 
    {
        public GroupDetails()
        {
            this.GroupAccess = new List<GroupAccess>();
        }

        public int Id { get; set; }

        [DisplayName("Nome")]
        public string Name { get; set; }

        [DisplayName("Aplicação")]
        public int ApplicationId { get; set; }

        public Application Application { get; set; }
        public List<GroupAccess> GroupAccess { get; set; }

    }
}
