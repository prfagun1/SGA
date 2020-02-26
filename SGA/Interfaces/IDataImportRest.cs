using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Interfaces
{
    public interface IDataImportRest
    {
        int SetRestValue(ApplicationRest applicationRest, out string json);
        void ImportRestConsultaUsuariosSistema(IQueryable<ApplicationRest> connectionRestIQueryable);
        bool ImportRestConsultaFuncionariosAfastados(IQueryable<ApplicationRest> connectionRestIQueryable);
        void ImportRestConsultaGruposPermissoes(IQueryable<ApplicationRest> connectionRestIQueryable);
        void ImportRestCentroDeCustos(IQueryable<ApplicationRest> connectionRestIQueryable);
        bool ImportRestRescicoes(IQueryable<ApplicationRest> connectionRestIQueryable);
        bool ImportRestNewEmployees(IQueryable<ApplicationRest> connectionRestIQueryable);
        string GetRestValue(ApplicationRest applicationRest);
    }
}
