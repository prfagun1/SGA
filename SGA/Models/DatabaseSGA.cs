using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{
    public class DatabaseSGA : BaseModel
    {
        [Required(ErrorMessage = "É necessário informar o campo nome")]
        [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "A descrição pode ter no máximo 1.000 caracteres.")]
        [Display(Name = "Descrição")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [StringLength(100, ErrorMessage = "O nome do banco pode ter no máximo 100 caracteres.")]
        [Display(Name = "Nome do banco")]
        public string DatabaseName { get; set; }

        [Required(ErrorMessage = "É necessário informar o campo nome do servidor")]
        [StringLength(100, ErrorMessage = "O nome do servidor pode ter no máximo 100 caracteres.")]
        [Display(Name = "Nome do servidor")]
        public string DatabaseServer { get; set; }

        [StringLength(100, ErrorMessage = "O nome do usuário de conexão 100 caracteres.")]
        [Display(Name = "Nome do usuário")]
        public string DatabaseUser { get; set; }


        [StringLength(255, ErrorMessage = "A senha pode ter no máximo 255 caracteres.")]
        [Display(Name = "Senha")]
        [DataType(DataType.Password)]
        public string DatabasePassword { get; set; }

        [Required(ErrorMessage = "É necessário informar a porta")]
        [Display(Name = "Porta")]
        public int Port { get; set; }

        [Display(Name = "Tipo de conexão")]
        [Required(ErrorMessage = "É necessário informar o tipo de conexão")]
        public int DatabaseTypeId { get; set; }


        [StringLength(1000, ErrorMessage = "A string de conexão pode ter no máximo 1.000 caracteres.")]
        [Display(Name = "String de conexão")]
        [DataType(DataType.MultilineText)]
        public string ConnectionString { get; set; }

        [Required(ErrorMessage = "É necessário informar se está ativo")]
        [Display(Name = "Ativo")]
        public EnumSGA.Status Enable { get; set; }


        [DisplayName("Ambiente")]
        [Required(ErrorMessage = "É necessário informar o ambiente")]
        public virtual int EnvironmentId { get; set; }

        [DisplayName("Ambiente")]
        public virtual Environment Environment { get; set; }

        [DisplayName("Tipo do banco de dados")]
        public virtual DatabaseType DatabaseType { get; set; }
    }
}
