
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SGA.Models
{
    public class UserAccess 
    {

        [DisplayName("Aplicação")]
        public int ApplicationId { get; set; }

        public int UserDetailsId { get; set; }

        [DisplayName("Aplicação")]
        public virtual Application Application { get; set; }

        public virtual UserDetails UserDetails { get; set; }

        [DisplayName("Grupos de segurança")]
        public GroupDetails GroupDetails { get; set; }

        [DisplayName("Grupos de segurança")]
        [Required]
        public int GroupDetailsId { get; set; }
    }
}
