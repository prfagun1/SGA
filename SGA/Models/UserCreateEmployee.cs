using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{

    public class UserCreateEmployee : BaseModel
    {
        [DisplayName("Nome completo")]
        public string FullName { get; set; }

        [DisplayName("Usuário")]
        public string Username { get; set; }

        [DisplayName("Crachá")]
        public string EmployeeId { get; set; }


        [DisplayName("ID Metadados")]
        public int? MetadadosId { get; set; }

        [DisplayName("Setor")]
        public string Department { get; set; }

        [DisplayName("CC")]
        public int CC { get; set; }

        [DisplayName("Função")]
        public string JobRole { get; set; }

        [DisplayName("Senha")]
        public string Password { get; set; }

        [DisplayName("Data de admissão")]
        public DateTime AdmissionDate { get; set; }

        [DisplayName("Chave Registro")]
        public string ChaveRegistro { get; set; }

        [DisplayName("Metadados Status")]
        public bool MetadadosStatus { get; set; }

        [DisplayName("Ativo")]
        public EnumSGA.UserCreateEmployeeStatus Status { get; set; }

    }
}
