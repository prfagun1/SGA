using SGA.Models;
using SGA.Repositories;

namespace SGA.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<ApplicationAD> ApplicationADRepository { get; }
        IRepository<Application> ApplicationRepository { get; }
        IRepository<ApplicationRest> ApplicationRestRepository { get; }
        IRepository<ApplicationSQL> ApplicationSQLRepository { get; }
        IRepository<ApplicationType> ApplicationTypeRepository { get; }
        IRepository<DatabaseSGA> DatabaseSGARepository { get; }
        IRepository<DatabaseType> DatabaseTypeRepository { get; }
        IRepository<Environment> EnvironmentRepository { get; }
        IRepository<GroupAccess> GroupAccessRepository { get; }
        IRepository<GroupDetails> GroupDetailsRepository { get; }
        IRepository<Ldap> LdapRepository { get; }
        IRepository<Log> LogRepository { get; }
        IRepository<Parameter> ParameterRepository { get; }
        IRepository<PermissionGroup> PermissionGroupRepository { get; }
        IRepository<UserAccess> UserAccessRepository { get; }
        IRepository<UserDetails> UserDetailsRepository { get; }
        IRepository<UserHR> UserHRRepository { get; }
        IRepository<Schedule> ScheduleRepository { get; }
        IRepository<CC> CCRepository { get; }
        IRepository<UserCreateEmployee> UserCreateEmployeeRepository { get; }
        ILogCustomRepository LogCustomRepository { get; }

        void Save();

        void Dispose();

    }
}
