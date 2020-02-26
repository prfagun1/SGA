using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Interfaces
{
    public interface IDataImportSQL{
        void ImportSQLConsultaGruposPermissoes(IQueryable<ApplicationSQL> connectionSQLIQueryable);
        void ImportSQLListagemDetalhesUsuarios(IQueryable<ApplicationSQL> connectionSQLIQueryable);
        void ImportSQLConsultaUsuariosSistema(IQueryable<ApplicationSQL> connectionSQLIQueryable);
    }
}
