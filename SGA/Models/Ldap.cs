using System.ComponentModel.DataAnnotations;

namespace SGA.Models
{
    public class Ldap : BaseModel
    {
        [Display(Name = "Servidor")]
        [Required(ErrorMessage = "É preciso informar o domínio")]
        [MaxLength(255, ErrorMessage = "Dominio deve ter no máximo 255 caracteres")]
        public string Domain { get; set; }


        [Display(Name = "Usuário de conexão")]
        [Required(ErrorMessage = "É preciso informar o usuário de conexão")]
        [MaxLength(255, ErrorMessage = "Usuário de conexã deve ter no máximo 255 caracteres")]
        public string BindUser { get; set; }

        [Display(Name = "Senha de conexão com LDAP")]
        [Required(ErrorMessage = "É preciso informar a senha de conexão")]
        [DataType(DataType.Password)]
        [MaxLength(255, ErrorMessage = "Senha deve ter no máximo 255 caracteres")]
        public string BindPassword { get; set; }


        [Display(Name = "Ativo")]
        [Required(ErrorMessage = "É necessário informar o status")]
        public EnumSGA.Status Enable { get; set; }
    }
}
