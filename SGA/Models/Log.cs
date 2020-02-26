using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{


    public class Log : BaseModel
    {
        [Display(Name = "Tipo")]
        public EnumSGA.LogType LogType { get; set; }

        [Display(Name = "Local")]
        public string Description { get; set; }

        [Display(Name = "Mensagem")]
        public string Message { get; set; }

    }
}

