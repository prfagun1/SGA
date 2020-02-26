using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace SGA.Models
{
    public class ApplicationRest : BaseModel
    {
        [DisplayName("Nome")]
        [Required(ErrorMessage = "É necessário informar um nome")]
        public string Name { get; set; }

        [DisplayName("Descrição")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Aplicação")]
        [Required(ErrorMessage = "É necessário selecionar uma aplicação")]
        public int ApplicationId { get; set; }

        [DisplayName("Tipo")]
        [Required(ErrorMessage = "É necessário informar um nome")]
        public virtual int ApplicationTypeId { get; set; }

        //0 = get   1 = post
        [DisplayName("GET/POST")]
        [Required(ErrorMessage = "É necessário informar o tipo de conexão")]
        public EnumSGA.RestType RestType { get; set; }

        [DisplayName("URL")]
        public string URL { get; set; }

        [DisplayName("API")]
        public string API { get; set; }

        [DisplayName("Header")]
        [DataType(DataType.MultilineText)]
        public string Header { get; set; }

        [DisplayName("Json")]
        [DataType(DataType.MultilineText)]
        public string Json { get; set; }

        [DisplayName("Username")]
        public string Username { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [DisplayName("Utilizar MD5 para Mirth")]
        [Required(ErrorMessage = "É necessário informar se usa Mirth Connect")]
        public bool MD5 { get; set; }
        
        [DisplayName("Ativo")]
        [Required(ErrorMessage = "É necessário informar o status")]
        public EnumSGA.Status Enable { get; set; }
        
        [DisplayName("Aplicação")]
        public virtual Application Application { get; set; }

        [DisplayName("Tipo")]
        public virtual ApplicationType ApplicationType { get; set; }
    }
}
