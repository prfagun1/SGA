using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{
    public class Environment : BaseModel
    {
        [DisplayName("Nome")]
        [Required(ErrorMessage = "É necessário informar um nome")]
        public String Name { get; set; }

        [DisplayName("Ativo")]
        public EnumSGA.Status Enable { get; set; }

    }
}
