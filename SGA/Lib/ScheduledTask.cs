using SGA.Models;
using Microsoft.Extensions.DependencyInjection;
using SGA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SGA.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace SGA.Lib
{
    public class ScheduledTask
    {

        private IDataWrite _dataWrite;
        private IAuthentication _authentication;
        private IUserHelper _userHelper;
        private readonly string LogDescription = "ScheduledTask";
        private SGAContext _context;
        private LogContext _logContext;
        private DbContextOptions<SGAContext> _options;
        private readonly IHttpContextAccessor _contextAcessor;

        public ScheduledTask(IServiceProvider serviceProvider)
        {
            IServiceScope serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            _dataWrite = serviceScope.ServiceProvider.GetService<IDataWrite>();
            _authentication = serviceScope.ServiceProvider.GetService<IAuthentication>();
            _userHelper = serviceScope.ServiceProvider.GetService<IUserHelper>();
            _logContext = serviceScope.ServiceProvider.GetService<LogContext>();
            _context = serviceScope.ServiceProvider.GetService<SGAContext>();
            _options = serviceScope.ServiceProvider.GetService<DbContextOptions<SGAContext>>();
            _contextAcessor = serviceScope.ServiceProvider.GetService<IHttpContextAccessor>(); ;

        }

        public void Start()
        {
            while (true)
            {
                try
                {
                    var filter = new List<Expression<Func<Schedule, bool>>>();
                    filter.Add(x => x.Enable == EnumSGA.Status.Enabled);


                    List<Schedule> scheduleList;
                    using (SGAContext ctx = new SGAContext(_options))
                    {
                        using (var _iuw = new UnitOfWork(ctx, _logContext, _contextAcessor))
                        {
                            scheduleList = _iuw.ScheduleRepository.GetList(filter).ToList();
                        }

                    }


                    foreach (var schedule in scheduleList)
                    {
                        schedule.LastTest = DateTime.Now;
                        TimeSpan time = DateTime.Now.TimeOfDay;

                        var difference = (time - schedule.Time).TotalMinutes;
                        DateTime date = DateTime.Parse(DateTime.Now.ToString());

                        //Teste de execução do job
                        //System.IO.File.WriteAllText(@"c:\temp\log-execucao.txt", "Data: " + DateTime.Now + " - Difference: " + difference + "\n");

                        if (difference > 0 && difference < 1)
                        {
                            if (schedule.Type == EnumSGA.ScheduleType.ImportUsersAndGrups)
                            {
                                Thread importUsersAndGrupsThread = new Thread(() => ImportUsersAndGroups(schedule, date));
                                importUsersAndGrupsThread.Start();
                                UpdateSchedule(schedule, date);
                                GC.Collect();
                            }

                            if (schedule.Type == EnumSGA.ScheduleType.UserEnableDisable)
                            {
                                Thread userEnableDisable = new Thread(() => UserEnableDisable(schedule, date));
                                userEnableDisable.Start();
                                UpdateSchedule(schedule, date);
                                GC.Collect();
                            }
                        }

                        UpdateScheduleTest(schedule, date);
                    }

                    Thread.Sleep(60000);
                }
                catch (Exception e)
                {
                    System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro: " + e.ToString() + "\n");
                }
            }

        }

        private void UpdateScheduleTest(Schedule schedule, DateTime date) {
            using (SGAContext ctx = new SGAContext(_options))
            {
                using (var _iuw = new UnitOfWork(ctx, _logContext, _contextAcessor))
                {
                    var scheduleUpdated = _iuw.ScheduleRepository.Get(x => x.Id == schedule.Id);
                    scheduleUpdated.LastTest = date;
                    _iuw.ScheduleRepository.Update(scheduleUpdated);
                    _iuw.Save();
                }

            }
        }

        private void UpdateSchedule(Schedule schedule, DateTime date)
        {
            using (SGAContext ctx = new SGAContext(_options))
            {
                using (var _iuw = new UnitOfWork(ctx, _logContext, _contextAcessor))
                {
                    var scheduleUpdated = _iuw.ScheduleRepository.Get(x => x.Id == schedule.Id);
                    scheduleUpdated.LastExecution = date;
                    _iuw.ScheduleRepository.Update(scheduleUpdated);
                    _iuw.Save();
                }

            }
        }

        private void ImportUsersAndGroups(Schedule schedule, DateTime date)
        {
            using (SGAContext ctx = new SGAContext(_options))
            {
                using (var iuw = new UnitOfWork(ctx, _logContext, _contextAcessor))
                {
                    DataImportAD dataImportAD = new DataImportAD(iuw);
                    DataImportSQL dataImportSQL = new DataImportSQL(iuw);
                    DataImportRest dataImportRest = new DataImportRest(iuw);

                    DataImport dataImport = new DataImport(iuw ,dataImportAD, dataImportSQL, dataImportRest);

                    dataImport.ImportAll();

                    schedule.LastExecution = date;
                    iuw.ScheduleRepository.Update(schedule);
                    iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Tarefa agendada nome ImportUsersAndGroups executada.");

                    dataImportAD = null;
                    dataImportSQL = null;
                    dataImportRest = null;
                    dataImport = null;

                }
            }

        }

        private void UserEnableDisable(Schedule schedule, DateTime date)
        {

            using (SGAContext ctx = new SGAContext(_options))
            {
                using (var iuw = new UnitOfWork(ctx, _logContext, _contextAcessor))
                {
                    DataImportAD dataImportAD = new DataImportAD(iuw);
                    DataImportSQL dataImportSQL = new DataImportSQL(iuw);
                    DataImportRest dataImportRest = new DataImportRest(iuw);

                    DataImport dataImport = new DataImport(iuw, dataImportAD, dataImportSQL, dataImportRest);
                    dataImport.ImportUserHRList();

                    DataWrite dataWrite = new DataWrite(iuw, dataImport, dataImportRest);

                    dataWrite.EnableDisableUsers();
                    iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Tarefa agendada nome UserEnableDisable executada.");

                    dataImportAD = null;
                    dataImportSQL = null;
                    dataImportRest = null;
                    dataImport = null;
                    dataWrite = null;
                }

            }

        }
    }

}
