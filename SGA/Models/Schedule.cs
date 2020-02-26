using System;
using System.ComponentModel.DataAnnotations;

namespace SGA.Models
{
    public class Schedule : BaseModel
    {
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "É preciso informar o nome")]
        [MaxLength(255, ErrorMessage = "O nome deve ter no máximo 255 caracteres")]
        public string Name { get; set; }

        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Display(Name = "Horário")]
        [Required(ErrorMessage = "É preciso informar a data inicial do agendamento")]
        public TimeSpan Time { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "É necessário informar o tipo")]
        public EnumSGA.ScheduleType Type { get; set; }

        [Display(Name = "Ativo")]
        [Required(ErrorMessage = "É necessário informar o status")]
        public EnumSGA.Status Enable { get; set; }

        [Display(Name = "Último teste")]
        public DateTime? LastTest { get; set; }

        [Display(Name = "Última execução")]
        public DateTime? LastExecution { get; set; }
    }
}
