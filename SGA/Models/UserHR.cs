using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{


    public class UserHR
    {
        public UserHR()
        {
            this.UserHRApplication = new List<UserHRApplication>();
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

        [DisplayName("Data alteração")]
        public DateTime ChangeDate { get; set; }

        [DisplayName("Data de rescição/Afastamento")]
        public DateTime MetadadosDate { get; set; }

        [DisplayName("Data de Volta")]
        public DateTime? MetadadosDateReturn { get; set; }

        [DisplayName("Chave Registro")]
        public string ChaveRegistro { get; set; }

        [DisplayName("Metadados Status")]
        public bool MetadadosStatus { get; set; }

        public EnumSGA.UserHRStatusHR  StatusRH { get; set; }

        public EnumSGA.UserHRStatusLocal StatusLocal { get; set; }

        public virtual ICollection<UserHRApplication> UserHRApplication { get; set; }
    }
}
