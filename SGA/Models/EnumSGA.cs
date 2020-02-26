using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{
    public class EnumSGA
    {
        public enum Status
        {
            Disabled,
            Enabled,
            All
        }

        public enum UserCreateEmployeeStatus
        {
            OK,
            Pendind,
        }

        public enum LogType
        {
            Info,
            Error
        }

        public enum AccessType
        {
            Administration,
            Reports,
            Logs,
            UserManagementRead,
            UserManagementWrite,
            Manager,
            HR,
            Public
        }

        public enum ScheduleType
        {
            UserEnableDisable,
            ImportUsersAndGrups
        }

        public enum UserHRStatusLocal
        {
            OK,
            Pendente,
            Afastado
        }

        public enum UserHRStatusHR
        {
            Afastado,
            Desligado,
            Voltando
        }

        public static UserHRStatusHR GetUserHRStatusHR(string userHRStatusHR) {

            switch (userHRStatusHR) {
                case "Afastado": return UserHRStatusHR.Afastado;
                case "Desligado": return UserHRStatusHR.Desligado;
                case "Voltando": return UserHRStatusHR.Voltando;
            }

            return UserHRStatusHR.Desligado;
        }

        public enum ConnectionType
        {
            ConsultaUsuariosSistema = 1,
            ListagemDetalhesUsuarios = 2,
            ConsultaGruposPermissoes = 3,
            ConsultaFuncionariosAfastados = 4,
            DesativaUsuariosSistema = 5,
            AtivaUsuariosSistema = 6,
            ConsultaCentroDeCustos = 7,
            ConsultaRescisoes = 8,
            ConsultaAdmissoess = 9,
            CriarUsuarios = 10
        }

        public enum ConnectionProcedure
        {
            ListagemDetalhesUsuarios,
            ImportaDadosUsuariosGrupos,
            AtivaDesativaUsuarios,
        }

        public enum DatabaseCommand
        {
            Select,
            Update
        }

        public enum RestType
        {
            GET,
            POST
        }
    }
}
