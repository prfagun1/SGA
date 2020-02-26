using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{
    public class Parameter : BaseModel
    {
        [Display(Name = "Caminho do arquivo de log")]
        [Required(ErrorMessage = "É preciso informar o caminho do log em caso de erros de conexão ao banco.")]
        [MinLength(1, ErrorMessage = "Nome deve ter no mínimo  caracter.")]
        [MaxLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres.")]
        public string LogErrorPath { get; set; }


        [Display(Name = "Nível do log")]
        [Required(ErrorMessage = "É preciso informar o nível do log.")]
        public EnumSGA.LogType LogLevelApplication { get; set; }

        [Display(Name = "Usuário administrativo")]
        [Required(ErrorMessage = "É preciso informar o nome.")]
        [MaxLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres.")]
        public string AdminUser { get; set; }

        [Display(Name = "Senha do usuário administrativo")]
        [Required(ErrorMessage = "É preciso informar a senha do usuário.")]
        [DataType(DataType.Password)]
        [MaxLength(500, ErrorMessage = "Nome deve ter no máximo 500 caracteres.")]
        public string AdminPassword { get; set; }

        [Display(Name = "Itens por página")]
        [Required(ErrorMessage = "É preciso informar a quantidade de itens que serão exibidos por página.")]
        public int ItensPage { get; set; }
        
        [Display(Name = "Tempo de retenção de logs em dias")]
        [Required(ErrorMessage = "É preciso informar o tempo de retenção dos logs em dias.")]
        public int LogsRetentionTime { get; set; }


        [Display(Name = "Valida usuário - URL API")]
        //[Required(ErrorMessage = "É preciso informar o nome do servidor.")]
        [MaxLength(100, ErrorMessage = "A URL deve ter no máximo 100 caracteres.")]
        public string ValidaUsuarioURL { get; set; }

        [Display(Name = "Valida usuário - Json")]
        //[Required(ErrorMessage = "É preciso informar a senha do usuário.")]
        [MaxLength(255, ErrorMessage = "A String Json deve ter no máximo 255 caracteres.")]
        public string ValidaUsuarioJson { get; set; }


        [Display(Name = "Lista de exclusão para não desativar usuários afastados")]
        //[Required(ErrorMessage = "É preciso informar o select.")]
        [MaxLength(1000, ErrorMessage = "A lista deve ter no máximo 10000 caracteres.")]
        public string ExclusionListAfastamento { get; set; }


        [Display(Name = "Valida usuário - Usuário API")]
        //[Required(ErrorMessage = "É preciso informar o select.")]
        [MaxLength(100, ErrorMessage = "Usuário API deve ter no máximo 100 caracteres.")]
        public string ValidaUsuarioUsername { get; set; }

        [Display(Name = "Valida usuário - Senha API")]
        //[Required(ErrorMessage = "É preciso informar a senha do usuário.")]
        [DataType(DataType.Password)]
        [MaxLength(100, ErrorMessage = "Senha API deve ter no máximo 100 caracteres.")]
        public string ValidaUsuarioPassword { get; set; }

    }
}
