using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace SGA.Models
{
    public class ApplicationSQL : BaseModel
    {
        [DisplayName("Nome")]
        [Required(ErrorMessage = "É necessário informar um nome")]
        public string Name { get; set; }

        [DisplayName("Descrição")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Tipo SQL")]
        [Required(ErrorMessage = "É necessário informar um tipo de SQL")]
        public int ApplicationTypeId { get; set; }

        [DisplayName("SQL")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "É necessário informar uma instrução SQL")]
        public string SQL { get; set; }

        [DisplayName("Banco de dados")]
        [Required(ErrorMessage = "É necessário selecionar um banco de dados")]
        public int DatabaseSGAId { get; set; }
        
        [DisplayName("Aplicação")]
        [Required(ErrorMessage = "É necessário selecionar uma aplicação")]
        public int ApplicationId { get; set; }

        [DisplayName("Ativo")]
        [Required(ErrorMessage = "É necessário informar o status")]
        public EnumSGA.Status Enable { get; set; }


        [DisplayName("Banco de dados")]
        public virtual DatabaseSGA DatabaseSGA { get; set; }

        [DisplayName("Tipo SQL")]
        public virtual ApplicationType ApplicationType { get; set; }

        [DisplayName("Aplicação")]
        public virtual Application Application { get; set; }
    }
}
