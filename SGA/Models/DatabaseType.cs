using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{
    public class DatabaseType : BaseModel
    {
        [DisplayName("Nome")]
        [Required(ErrorMessage = "É necessário informar um nome")]
        public String Name { get; set; }

        [DisplayName("Descrição")]
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }

        [DisplayName("Ativo")]
        public Models.EnumSGA.Status Enable { get; set; }
    }
}
