using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{
    public class UserDetails
    {
        public UserDetails()
        {
            this.UserAccess = new List<UserAccess>();
            //this.GroupAccess = new List<GroupAccess>();
        }

        public int Id { get; set; }

        [DisplayName("Usuário")]
        public string Username { get; set; }

        [DisplayName("Nome completo")]
        public string FullName { get; set; }

        [DisplayName("Função")]
        public string JobRole { get; set; }

        [DisplayName("Setor")]
        public string Department { get; set; }

        [DisplayName("Centro de custo")]
        public int? CC { get; set; }

        public virtual IList<UserAccess> UserAccess { get; set; }
        //public virtual IList<GroupAccess> GroupAccess { get; set; }
    }
}
