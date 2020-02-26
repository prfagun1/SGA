using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{
    public class ApplicationAD : BaseModel
    {
        [DisplayName("Nome")]
        [Required(ErrorMessage = "É necessário informar um nome")]
        public string Name { get; set; }

        [DisplayName("Descrição")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Tipo grupo")]
        public int ApplicationTypeId { get; set; }

        [DisplayName("Grupos")]
        [DataType(DataType.MultilineText)]
        public string Groups { get; set; }

        [DisplayName("Aplicação")]
        public int ApplicationId { get; set; }

        [DisplayName("Ativo")]
        public EnumSGA.Status Enable { get; set; }

        [DisplayName("Tipo")]
        public virtual ApplicationType ApplicationType { get; set; }

        [DisplayName("Aplicação")]
        public virtual Application Application { get; set; }
    }
}
