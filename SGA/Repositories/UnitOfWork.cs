using Microsoft.AspNetCore.Http;
using SGA.Interfaces;
using SGA.Models;
using System;
using Environment = SGA.Models.Environment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SGA.Repositories
{
    public class UnitOfWork : IUnitOfWork,  IDisposable
    {
        private SGAContext _context = null;
        private LogContext _logContext = null;
        private readonly IHttpContextAccessor _contextAcessor;

        private Repository<ApplicationAD> applicationADRepository = null;
        private Repository<Application> applicationRepository = null;
        private Repository<ApplicationRest> applicationRestRepository = null;
        private Repository<ApplicationSQL> applicationSQLRepository = null;
        private Repository<ApplicationType> applicationTypeRepository = null;
        private Repository<DatabaseSGA> databaseSGARepository = null;
        private Repository<DatabaseType> databaseTypeRepository = null;
        private Repository<Environment> environmentRepository = null;
        private Repository<GroupAccess> groupAccessRepository = null;
        private Repository<GroupDetails> groupDetailsRepository = null;
        private Repository<Ldap> ldapRepository = null;
        private Repository<Log> logRepository = null;
        private Repository<Parameter> parameterRepository = null;
        private Repository<PermissionGroup> permissionGroupRepository = null;
        private Repository<UserAccess> userAccessRepository = null;
        private Repository<UserDetails> userDetailsRepository = null;
        private Repository<UserHR> userHRRepository = null;
        private Repository<Schedule> scheduleRepository = null;
        private Repository<CC> ccRepository = null;
        private Repository<UserCreateEmployee> userCreateEmployeeRepository = null;
        

        private LogCustomRepository logCustomRepository = null;

        public UnitOfWork(SGAContext context, LogContext logContext, IHttpContextAccessor contextAcessor)
        {
            _context = context;
            _logContext = logContext;
            _contextAcessor = contextAcessor;

        }


        public void Save()
        {
            _context.SaveChanges();
        }

        public IRepository<UserCreateEmployee> UserCreateEmployeeRepository
        {
            get { return userCreateEmployeeRepository = new Repository<UserCreateEmployee>(_context); }
        }

        public IRepository<ApplicationAD> ApplicationADRepository
        {
            get { return applicationADRepository = new Repository<ApplicationAD>(_context); }
        }

        public IRepository<Application> ApplicationRepository
        {
            get { return applicationRepository = new Repository<Application>(_context); }
        }

        public IRepository<ApplicationRest> ApplicationRestRepository
        {
            get { return applicationRestRepository = new Repository<ApplicationRest>(_context); }
        }

        public IRepository<ApplicationSQL> ApplicationSQLRepository
        {
            get { return applicationSQLRepository = new Repository<ApplicationSQL>(_context); }
        }

        public IRepository<ApplicationType> ApplicationTypeRepository
        {
            get { return applicationTypeRepository = new Repository<ApplicationType>(_context); }
        }

        public IRepository<DatabaseSGA> DatabaseSGARepository
        {
            get { return databaseSGARepository = new Repository<DatabaseSGA>(_context); }
        }

        public IRepository<DatabaseType> DatabaseTypeRepository
        {
            get { return databaseTypeRepository = new Repository<DatabaseType>(_context); }
        }

        public IRepository<Environment> EnvironmentRepository
        {
            get { return environmentRepository = new Repository<Environment>(_context); }
        }

        public IRepository<GroupAccess> GroupAccessRepository
        {
            get { return groupAccessRepository = new Repository<GroupAccess>(_context); }
        }

        public IRepository<GroupDetails> GroupDetailsRepository
        {
            get { return groupDetailsRepository = new Repository<GroupDetails>(_context); }
        }

        public IRepository<Ldap> LdapRepository
        {
            get { return ldapRepository = new Repository<Ldap>(_context); }
        }

        public IRepository<Log> LogRepository
        {
            get { return logRepository = new Repository<Log>(_context); }
        }
        public ILogCustomRepository LogCustomRepository
        {
            get { return logCustomRepository = new LogCustomRepository(_logContext, _contextAcessor, this); }
        }

        public IRepository<Parameter> ParameterRepository
        {
            get { return parameterRepository = new Repository<Parameter>(_context); }

        }

        public IRepository<PermissionGroup> PermissionGroupRepository
        {
            get { return permissionGroupRepository = new Repository<PermissionGroup>(_context); }

        }

        public IRepository<UserAccess> UserAccessRepository
        {
            get { return userAccessRepository = new Repository<UserAccess>(_context); }
        }

        public IRepository<UserDetails> UserDetailsRepository
        {
            get { return userDetailsRepository = new Repository<UserDetails>(_context); }
        }

        public IRepository<UserHR> UserHRRepository
        {
            get { return userHRRepository = new Repository<UserHR>(_context); }
        }

        public IRepository<Schedule> ScheduleRepository
        {
            get { return scheduleRepository = new Repository<Schedule>(_context); }
        }

        public IRepository<CC> CCRepository
        {
            get { return ccRepository = new Repository<CC>(_context); }
        }

        private bool disposed = false;


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}


/*
public ApplicationRepository ApplicationRepository
{
    get
    {
        return new ApplicationRepository(_context);
    }
}*/
