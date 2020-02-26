using Microsoft.AspNetCore.Mvc.Rendering;
using SGA.Models;

namespace SGA.Lib
{
    public class ControllersHelper
    {
        public static SelectList GetSelectList(EnumSGA.Status? status)
        {
            return new SelectList(new[] {
                new { ID = EnumSGA.Status.Enabled, Name = "Sim" },
                new { ID = EnumSGA.Status.Disabled, Name = "Não" },
            }, "ID", "Name", status);

          }

        public static SelectList GetSelectList(EnumSGA.AccessType? accessType) { 
            return new SelectList(new[] {
                new { ID = EnumSGA.AccessType.Administration, Name = "Administração da ferramenta" },
                new { ID = EnumSGA.AccessType.Reports, Name = "Relatórios" },
                new { ID = EnumSGA.AccessType.Logs, Name = "Logs" },
                new { ID = EnumSGA.AccessType.UserManagementRead, Name = "Gerenciamento de usuário - Leitura" },
                new { ID = EnumSGA.AccessType.UserManagementWrite, Name = "Gerenciamento de usuários - Escrita" },
                new { ID = EnumSGA.AccessType.Manager, Name = "Gestão" },
                new { ID = EnumSGA.AccessType.HR, Name = "Recursos Humanos" },
            }, "ID", "Name", accessType);
        }

 
        public static SelectList GetSelectList(EnumSGA.LogType? logTytpe)
        {
            return new SelectList(new[] {
                new { ID = EnumSGA.LogType.Error, Name = "Somente erros" },
                new { ID = EnumSGA.LogType.Info, Name = "Auditoria" },
            }, "ID", "Name", logTytpe);
        }


        public static SelectList GetSelectList(EnumSGA.ScheduleType? scheduleType)
        {
            return new SelectList(new[] {
                new { ID = EnumSGA.ScheduleType.ImportUsersAndGrups, Name = "Importar usuários e grupos" },
                new { ID = EnumSGA.ScheduleType.UserEnableDisable, Name = "Desativar e reativar usuários" },
            }, "ID", "Name", scheduleType);
        }

        public static SelectList GetSelectList(EnumSGA.UserHRStatusHR? userHRStatusHR)
        {
            return new SelectList(new[] {
                new { ID = EnumSGA.UserHRStatusHR.Afastado, Name = "Afastado" },
                new { ID = EnumSGA.UserHRStatusHR.Desligado, Name = "Desligado" },
                new { ID = EnumSGA.UserHRStatusHR.Voltando, Name = "Voltando" },
            }, "ID", "Name", userHRStatusHR);
        }

        public static SelectList GetSelectList(EnumSGA.UserHRStatusLocal? userHRStatusLocal)
        {
            return new SelectList(new[] {
                new { ID = EnumSGA.UserHRStatusLocal.OK, Name = "OK" },
                new { ID = EnumSGA.UserHRStatusLocal.Pendente, Name = "Pendente" },
            }, "ID", "Name", userHRStatusLocal);
        }
    }
}
