using Microsoft.AspNetCore.Http;
using SGA.Interfaces;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SGA.Repositories
{
    public class LogCustomRepository : ILogCustomRepository
    {
        private readonly LogContext _logContext;
        private readonly IHttpContextAccessor _contextAcessor;
        private readonly IUnitOfWork _iuw;

        public LogCustomRepository(LogContext logContext, IHttpContextAccessor contextAcessor, IUnitOfWork iuw)
        {
            _logContext = logContext;
            _contextAcessor = contextAcessor;
            _iuw = iuw;
        }

        public void SaveLogApplicationMessage(string description, string message)
        {
            Log messageLog = new Log();
            messageLog.LogType = EnumSGA.LogType.Info;
            SaveLogApplication(messageLog, description, message);
        }

        public void SaveLogApplicationError(string description, string message)
        {
            Log messageLog = new Log();
            messageLog.LogType = EnumSGA.LogType.Error;
            SaveLogApplication(messageLog, description, message);

        }

        private void SaveLogApplication(Log messageLog, string description, string message)
        {
            try
            {
                Parameter parameter = _iuw.ParameterRepository.Get(x => x.Id == 1);
                if (parameter == null) {
                    SaveLogError(new NullReferenceException("Não foram cadastrados os parâmetros no sistema"), false);
                }


                if (parameter.LogLevelApplication == EnumSGA.LogType.Error && messageLog.LogType == EnumSGA.LogType.Info) {
                    return;
                }

                string username = "";
                if (_contextAcessor.HttpContext == null)
                {
                    username = "Service";
                }
                else
                {
                    username = _contextAcessor.HttpContext.User.Identity.Name ?? "Service";
                }

                messageLog.Description = description;
                messageLog.ChangeDate = DateTime.Now;
                messageLog.User = username;
                messageLog.Message = message;

                _logContext.Add(messageLog);
                _logContext.SaveChanges();

            }
            catch (Exception e)
            {
                SaveLogError(e);
            }
        }


        private void SaveLogError(Exception e, bool parameterOK = true)
        {
            string logFile = "";
            try
            {

                if (parameterOK)
                {
                    Parameter parameter = _iuw.ParameterRepository.Get(x => x.Id == 1);
                    logFile = parameter.LogErrorPath;
                }

            }
            catch
            {
                logFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
                logFile = logFile.Substring(0, logFile.LastIndexOf("\\"));
                logFile = logFile + @"\\error.log";
            }

            try
            {
                System.IO.File.AppendAllText(logFile, DateTime.Now.ToString() + " - " + e.ToString() + System.Environment.NewLine);
            }
            catch { }
        }

    }
}
