using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace SGA.Models
{
    public class Application : BaseModel
    {
        [DisplayName("Nome")]
        [Required(ErrorMessage = "É necessário informar um nome")]
        public string Name { get; set; }

        [DisplayName("Descrição")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Ambiente")]
        public int EnvironmentId { get; set; }

        [DisplayName("Ativo")]
        public EnumSGA.Status Enable { get; set; }


        [DisplayName("Ambiente")]
        public virtual Environment Environment { get; set; }

        public virtual ICollection<ApplicationSQL> ApplicationSQL { get; set; }

        public virtual ICollection<ApplicationRest> ApplicationRest { get; set; }

        public virtual ICollection<GroupDetails> GroupDetails { get; set; }

        public virtual ICollection<UserHRApplication> UserHRApplication { get; set; }
    }
}
