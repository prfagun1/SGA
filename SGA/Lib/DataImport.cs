using SGA.Interfaces;
using SGA.Models;
using SGA.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SGA.Lib
{
    public class DataImport : IDataImport
    {

        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "DataImport";
        private IDataImportAD _dataImportAD;
        private IDataImportSQL _dataImportSQL;
        private IDataImportRest _dataImportRest;

        public DataImport(IUnitOfWork iuw, IDataImportAD dataImportAD, IDataImportSQL dataImportSQL, IDataImportRest dataImportRest)
        {
            _iuw = iuw;
            _dataImportSQL = dataImportSQL;
            _dataImportRest = dataImportRest;
            _dataImportAD = dataImportAD;
        }

        private void TruncateTables()
        {
            try
            {
                _iuw.DatabaseSGARepository.SQLCommand("TRUNCATE Table useraccess");
                _iuw.DatabaseSGARepository.SQLCommand("DELETE FROM userdetails");
                _iuw.DatabaseSGARepository.SQLCommand("ALTER TABLE userdetails AUTO_INCREMENT = 1;");
                _iuw.DatabaseSGARepository.SQLCommand("DELETE FROM groupaccess");
                _iuw.DatabaseSGARepository.SQLCommand("ALTER TABLE groupaccess AUTO_INCREMENT = 1;");
                _iuw.DatabaseSGARepository.SQLCommand("DELETE FROM groupdetails");
                _iuw.DatabaseSGARepository.SQLCommand("ALTER TABLE groupdetails AUTO_INCREMENT = 1;");
                _iuw.DatabaseSGARepository.SQLCommand("TRUNCATE Table cc");


                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Tabelas limpas para iniciar o processo de importação.");
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao limpar tabelas antes de iniciar o processo de importação. " + e.Message);
            }
        }

        public void ImportAll()
        {
            _iuw.Save();
            TruncateTables();

            ImportSQL();
            ImportRest();
            ImportAD();
        }

        private void ImportRest()
        {
            var filter = new List<Expression<Func<ApplicationRest, bool>>>();
            filter.Add(x => x.Enable == EnumSGA.Status.Enabled && x.Application.Enable == EnumSGA.Status.Enabled);

            IQueryable<ApplicationRest> connectionRestIQueryable = _iuw.ApplicationRestRepository.GetList(filter, x => x.Application, x => x.ApplicationType);

            try
            {
                _dataImportRest.ImportRestConsultaUsuariosSistema(connectionRestIQueryable);
                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Finalizada a importação de usuários do sistema via REST.");
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao importar usuários do sistema via REST. " + e.Message);
            }
            try
            {
                _dataImportRest.ImportRestConsultaGruposPermissoes(connectionRestIQueryable);
                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Finalizada a importação de grupos do sistema via REST.");
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao importar grupos do sistema via REST. " + e.Message);
            }
            try
            {
                _dataImportRest.ImportRestCentroDeCustos(connectionRestIQueryable);
                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Finalizada a importação de centros de custo via REST.");
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao importar centros de custo via REST. " + e.Message);
            }
        }
        private void ImportAD()
        {
            var filter = new List<Expression<Func<ApplicationAD, bool>>>();
            filter.Add(x => x.Enable == EnumSGA.Status.Enabled && x.Application.Enable == EnumSGA.Status.Enabled);

            IQueryable<ApplicationAD> connectionADIQueryable = _iuw.ApplicationADRepository.GetList(filter, x => x.Application, x => x.ApplicationType);

            try
            {
                _dataImportAD.ImportADConsultaUsuariosSistema(connectionADIQueryable);
                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Finalizada a importação de usuários do AD.");
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao importar usuários do sistema via AD. " + e.Message);
            }
        }


        private void ImportSQL()
        {
            var filter = new List<Expression<Func<ApplicationSQL, bool>>>();
            filter.Add(x => x.Enable == EnumSGA.Status.Enabled && x.Application.Enable == EnumSGA.Status.Enabled);

            IQueryable<ApplicationSQL> connectionSQLIQueryable = _iuw.ApplicationSQLRepository.GetList(filter, x => x.DatabaseSGA, x => x.ApplicationType);

            try
            {
                //Teste
                _iuw.Save();
                _dataImportSQL.ImportSQLListagemDetalhesUsuarios(connectionSQLIQueryable);
                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Finalizada a importação de detalhes de usuários via SQL do processo ImportSQLListagemDetalhesUsuarios.");
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao importar detalhes de usuários via SQL do processo ImportSQLListagemDetalhesUsuarios. {e.Message}. Exceção interna {e.InnerException.ToString()}"  );
            }
            try
            {
                _dataImportSQL.ImportSQLConsultaUsuariosSistema(connectionSQLIQueryable);
                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Finalizada a importação de usuários do sistema via SQL do processo ImportSQLConsultaUsuariosSistema.");
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao importar usuários do sistema via SQL do processo ImportSQLConsultaUsuariosSistema. " + e.Message);
            }
            try
            {
                _dataImportSQL.ImportSQLConsultaGruposPermissoes(connectionSQLIQueryable);
                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Iniciada a importação de grupos e permissões via SQL do processo ImportSQLConsultaGruposPermissoes.");
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao importar grupos e permissões via SQL do processo ImportSQLConsultaGruposPermissoes. " + e.Message);
            }

        }


        public bool ImportUserHRList()
        {
            bool status;
            var filter = new List<Expression<Func<ApplicationRest, bool>>>();
            filter.Add(x => x.Enable == EnumSGA.Status.Enabled);

            IQueryable<ApplicationRest> connectionRestIQueryable = _iuw.ApplicationRestRepository.GetList(filter, x => x.Application, x => x.ApplicationType);

            status = _dataImportRest.ImportRestConsultaFuncionariosAfastados(connectionRestIQueryable);

            if (!status)
                return false;

            status = _dataImportRest.ImportRestRescicoes(connectionRestIQueryable);

            return status;
        }

        public bool ImportNewEmployees()
        {
            var filter = new List<Expression<Func<ApplicationRest, bool>>>();
            filter.Add(x => x.Enable == EnumSGA.Status.Enabled);

            IQueryable<ApplicationRest> connectionRestIQueryable = _iuw.ApplicationRestRepository.GetList(filter, x => x.Application, x => x.ApplicationType);

            bool status = _dataImportRest.ImportRestNewEmployees(connectionRestIQueryable);

            return status;
        }
    }
}
