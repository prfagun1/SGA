using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGA.Interfaces;
using SGA.Models;
using X.PagedList;
using static SGA.Models.EnumSGA;

namespace SGA.Controllers
{
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "Report";
        private readonly IDataWrite _dataWrite;

        public ReportController(IUnitOfWork iuw, IDataWrite dataWrite)
        {
            _iuw = iuw;
            _dataWrite = dataWrite;
        }


        [Authorize(Policy = "Report")]
        public async Task<IActionResult> UserCreateEmployee(int? page, string FullName, string Username, string EmployeeId, string JobRole, string Department, int? CC, string sortOrder, string sortOrderOld, DateTime? AdmissionDateStart, DateTime? AdmissionDateEnd )
        {

            try
            {
                int itensPages = _iuw.ParameterRepository.Get(x => x.Id == 1).ItensPage;
                var pageNumber = page ?? 1;


                var filter = new List<Expression<Func<UserCreateEmployee, bool>>>();


                if (!string.IsNullOrEmpty(FullName))
                {
                    filter.Add(x => x.FullName.Contains(FullName));
                }

                if (!string.IsNullOrEmpty(Username))
                {
                    filter.Add(x => x.Username.Contains(Username));
                }


                if (!string.IsNullOrEmpty(EmployeeId))
                {
                    filter.Add(x => x.EmployeeId.Contains(EmployeeId));
                }

                if (!string.IsNullOrEmpty(Department))
                {
                    filter.Add(x => x.Department.Contains(Department));
                }

                if (!string.IsNullOrEmpty(JobRole))
                {
                    filter.Add(x => x.JobRole.Contains(JobRole));
                }

                if (CC != null)
                {
                    filter.Add(x => x.CC == CC);
                }

                if (AdmissionDateStart != null)
                {
                    filter.Add(x => x.AdmissionDate >= AdmissionDateStart);
                }

                if (AdmissionDateEnd != null)
                {
                    filter.Add(x => x.AdmissionDate >= AdmissionDateEnd);
                }

                var entityList = _iuw.UserCreateEmployeeRepository.GetList(filter);


                if (sortOrder != null && sortOrderOld != null && sortOrder == sortOrderOld && !sortOrder.EndsWith("Desc") && pageNumber == 1)
                {
                    sortOrder += "Desc";
                }

                ViewBag.sortOrder = sortOrder;
                ViewBag.sortOrderOld = sortOrder;

                switch (sortOrder)
                {
                    case "Nome completo":
                        entityList = entityList.OrderBy(x => x.FullName);
                        break;
                    case "Nome completoDesc":
                        entityList = entityList.OrderByDescending(x => x.FullName);
                        break;
                    case "Usuário":
                        entityList = entityList.OrderBy(x => x.Username);
                        break;
                    case "UsuárioDesc":
                        entityList = entityList.OrderByDescending(x => x.Username);
                        break;
                    case "Senha":
                        entityList = entityList.OrderBy(x => x.EmployeeId);
                        break;
                    case "SenhaDesc":
                        entityList = entityList.OrderByDescending(x => x.EmployeeId);
                        break;
                    case "Crachá":
                        entityList = entityList.OrderBy(x => x.EmployeeId);
                        break;
                    case "CracháDesc":
                        entityList = entityList.OrderByDescending(x => x.EmployeeId);
                        break;
                    case "Setor":
                        entityList = entityList.OrderBy(x => x.Department);
                        break;
                    case "SetorDesc":
                        entityList = entityList.OrderByDescending(x => x.Department);
                        break;
                    case "Cargo":
                        entityList = entityList.OrderBy(x => x.JobRole);
                        break;
                    case "CargoDesc":
                        entityList = entityList.OrderByDescending(x => x.JobRole);
                        break;
                    case "Centro de custo":
                        entityList = entityList.OrderBy(x => x.CC);
                        break;
                    case "Centro de custoDesc":
                        entityList = entityList.OrderByDescending(x => x.CC);
                        break;
                    case "Data de admissão":
                        entityList = entityList.OrderBy(x => x.AdmissionDate);
                        break;
                    case "Data de admissãoDesc":
                        entityList = entityList.OrderByDescending(x => x.AdmissionDate);
                        break;
                    default:
                        entityList = entityList.OrderByDescending(x => x.AdmissionDate).ThenBy(x => x.FullName);
                        break;
                }

                ViewBag.FullName = FullName;
                ViewBag.Username = Username;
                ViewBag.JobRole = EmployeeId;
                ViewBag.JobRole = JobRole;
                ViewBag.Department = Department;
                ViewBag.CC = CC;
                ViewBag.AdmissionDateStart = AdmissionDateStart;
                ViewBag.AdmissionDateEnd = AdmissionDateEnd;


                ViewBag.UserCreateEmployee = await entityList.ToPagedListAsync(pageNumber, itensPages);

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Consulta realizada ao relatório UserCreateEmployee.");

                return View();

            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao exibir consulta UserCreateEmployee: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }

        }

        [Authorize(Policy = "Report") ]
        public async Task<IActionResult> UserApplication(int? page, string FullName, string Username, string JobRole, string Department, int? CC, int? ApplicationId, string Groups, string sortOrder, string sortOrderOld)
        {
            try
            {
                int itensPages = _iuw.ParameterRepository.Get(x => x.Id == 1).ItensPage;
                var pageNumber = page ?? 1;


                var filter = new List<Expression<Func<UserAccess, bool>>>();


                if (!string.IsNullOrEmpty(FullName))
                {
                    filter.Add(x => x.UserDetails.FullName.Contains(FullName));
                }

                if (!string.IsNullOrEmpty(Username))
                {
                    filter.Add(x => x.UserDetails.Username.Contains(Username));
                }

                if (!string.IsNullOrEmpty(Department))
                {
                    filter.Add(x => x.UserDetails.Department.Contains(Department));
                }

                if (!string.IsNullOrEmpty(JobRole))
                {
                    filter.Add(x => x.UserDetails.JobRole.Contains(JobRole));
                }

                if (CC != null)
                {
                    filter.Add(x => x.UserDetails.CC == CC);
                }

                if (ApplicationId != null)
                {
                    filter.Add(x => x.ApplicationId == ApplicationId);
                }

                if (!string.IsNullOrEmpty(Groups))
                {
                    filter.Add(x => x.GroupDetails.Name.Contains(Groups));
                }



                var entityList = _iuw.UserAccessRepository.GetList(filter, include: source => source
                    .Include(a => a.Application)
                    .Include(a => a.GroupDetails)
                    .ThenInclude(a => a.GroupAccess)
                    .Include(a => a.UserDetails)
                );


                if (sortOrder != null && sortOrderOld != null && sortOrder == sortOrderOld && !sortOrder.EndsWith("Desc") && pageNumber == 1)
                {
                    sortOrder += "Desc";
                }

                ViewBag.sortOrder = sortOrder;
                ViewBag.sortOrderOld = sortOrder;


                switch (sortOrder)
                {
                    case "Nome":
                        entityList = entityList.OrderBy(x => x.UserDetails.FullName);
                        break;
                    case "NomeDesc":
                        entityList = entityList.OrderByDescending(x => x.UserDetails.FullName);
                        break;
                    case "Username":
                        entityList = entityList.OrderBy(x => x.UserDetails.Username);
                        break;
                    case "UsernameDesc":
                        entityList = entityList.OrderByDescending(x => x.UserDetails.Username);
                        break;
                    case "Setor":
                        entityList = entityList.OrderBy(x => x.UserDetails.Department);
                        break;
                    case "SetorDesc":
                        entityList = entityList.OrderByDescending(x => x.UserDetails.Department);
                        break;
                    case "Cargo":
                        entityList = entityList.OrderBy(x => x.UserDetails.JobRole);
                        break;
                    case "CargoDesc":
                        entityList = entityList.OrderByDescending(x => x.UserDetails.JobRole);
                        break;
                    case "Centro de custo":
                        entityList = entityList.OrderBy(x => x.UserDetails.CC);
                        break;
                    case "Centro de custoDesc":
                        entityList = entityList.OrderByDescending(x => x.UserDetails.CC);
                        break;
                    case "Aplicação":
                        entityList = entityList.OrderBy(x => x.Application.Name);
                        break;
                    case "AplicaçãoDesc":
                        entityList = entityList.OrderByDescending(x => x.Application.Name);
                        break;
                    case "Grupo":
                        entityList = entityList.OrderBy(x => x.GroupDetails.Name);
                        break;
                    case "GrupoDesc":
                        entityList = entityList.OrderByDescending(x => x.GroupDetails.Name);
                        break;
                    default:
                        entityList = entityList.OrderBy(x => x.UserDetails.FullName).ThenBy(x => x.Application.Name);
                        break;
                }


                ViewBag.FullName = FullName;
                ViewBag.Username = Username;
                ViewBag.Department = Department;
                ViewBag.JobRole = JobRole;
                ViewBag.CC = CC;
                ViewBag.Groups = Groups;


                var applicationList = _iuw.ApplicationRepository.GetList(new List<Expression<Func<Models.Application, bool>>>()).OrderBy(x => x.Name);
                ViewBag.ApplicationId = new SelectList(applicationList, "Id", "Name", ApplicationId);
                ViewBag.UserAccess = await entityList.ToPagedListAsync(pageNumber, itensPages);

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Consulta realizada ao relatório UserApplication.");

                return View();

            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao exibir consulta UserApplication: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }

        }

        [Authorize(Policy = "Report")]
        public IEnumerable<string> GetGroupDetails(string GroupName, int ApplicationId)
        {
            try
            {
                GroupDetails groupDetail = _iuw.GroupDetailsRepository.Get(x => x.ApplicationId == ApplicationId && x.Name == GroupName);

                if (groupDetail == null)
                {
                    return null;
                }

                var filter = new List<Expression<Func<GroupAccess, bool>>>();
                filter.Add(x => x.GroupDetailsId == groupDetail.Id);

                List<GroupAccess> groupAccess = _iuw.GroupAccessRepository.GetList(filter: filter)
                    .Take(100)
                    .ToList();

                return groupAccess.Select(x => x.Permission).OrderBy(x => x);
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao exibir consulta GetGroupDetails: " + e.ToString());
                return null;
            }
        }

        [Authorize(Policy = "Report")]
        public IEnumerable<string> GetUserListFromGroup(string GroupName, int ApplicationId)
        {
            try
            {
                var filter = new List<Expression<Func<UserAccess, bool>>>();
                filter.Add(x => x.ApplicationId == ApplicationId);
                filter.Add(x => x.GroupDetails.Name.Contains(GroupName));

                var entityList = _iuw.UserAccessRepository.GetList(filter, include: source => source
                    .Include(a => a.UserDetails)
                )
                    .Where(x => !String.IsNullOrEmpty(x.UserDetails.Department) && !String.IsNullOrEmpty(x.UserDetails.FullName))
                    .Select(x => x.UserDetails.Department.ToString() + " - " + x.UserDetails.FullName.ToString()).ToList().OrderBy(x => x);

                return entityList;

            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao exibir consulta GetUserListFromGroup: " + e.ToString());
                return null;
            }
        }

        [Authorize(Policy = "Report")]
        public async Task<IActionResult> GroupApplication(int? page, int? ApplicationId, string Group, string sortOrder, string sortOrderOld)
        {

            try
            {

                int itensPages = _iuw.ParameterRepository.Get(x => x.Id == 1).ItensPage;
                var pageNumber = page ?? 1;

                var filter = new List<Expression<Func<GroupDetails, bool>>>();


                if (!string.IsNullOrEmpty(Group))
                {
                    filter.Add(x => x.Name.Contains(Group));
                }


                if (ApplicationId != null)
                {
                    filter.Add(x => x.ApplicationId == ApplicationId);
                }


                if (sortOrder != null && sortOrderOld != null && sortOrder == sortOrderOld && !sortOrder.EndsWith("Desc") && pageNumber == 1)
                {
                    sortOrder += "Desc";
                }

                var entityList = _iuw.GroupDetailsRepository.GetList(filter, include: source => source.Include(a => a.Application));

                ViewBag.sortOrder = sortOrder;
                ViewBag.sortOrderOld = sortOrder;


                switch (sortOrder)
                {
                    case "Aplicação":
                        entityList = entityList.OrderBy(x => x.Application.Name);
                        break;
                    case "AplicaçãoDesc":
                        entityList = entityList.OrderByDescending(x => x.Application.Name);
                        break;
                    case "Grupos":
                        entityList = entityList.OrderBy(x => x.Name);
                        break;
                    case "GruposDesc":
                        entityList = entityList.OrderByDescending(x => x.Name);
                        break;
                    default:
                        entityList = entityList.OrderBy(x => x.Application.Name).ThenBy(x => x.Name);
                        break;
                }


                ViewBag.Group = Group;


                var applicationList = _iuw.ApplicationRepository.GetList(new List<Expression<Func<Models.Application, bool>>>()).OrderBy(x => x.Name);
                ViewBag.ApplicationId = new SelectList(applicationList, "Id", "Name", ApplicationId);
                ViewBag.GroupDetails = await entityList.ToPagedListAsync(pageNumber, itensPages);

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Consulta realizada ao relatório GroupApplication.");

                return View();

            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao exibir consulta GroupApplication: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }

        }

        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> ManagerUserPermission(int? page, string FullName, string Username, string JobRole, string Department, int? CC)
        {

            try
            {
                int itensPages = _iuw.ParameterRepository.Get(x => x.Id == 1).ItensPage;
                var pageNumber = page ?? 1;


                var filter = new List<Expression<Func<UserDetails, bool>>>();


                if (!string.IsNullOrEmpty(FullName))
                {
                    filter.Add(x => x.FullName.Contains(FullName));
                }

                if (!string.IsNullOrEmpty(Username))
                {
                    filter.Add(x => x.Username.Contains(Username));
                }


                if (!string.IsNullOrEmpty(JobRole))
                {
                    filter.Add(x => x.JobRole.Contains(JobRole));
                }

                if (!string.IsNullOrEmpty(Department))
                {
                    filter.Add(x => x.Department.Contains(JobRole));
                }

                if (CC != null)
                {
                    filter.Add(x => x.CC == CC);
                }


                //Busca os funcionáriso de um gestor baseado no centro de custo
                var username = HttpContext.User.Identity.Name;
                var filterCC = new List<Expression<Func<CC, bool>>>();
                filterCC.Add(x => x.Username == username);
                List<int> ccsManager = _iuw.CCRepository.GetList(filterCC).Select(x => x.Id).ToList();

                filter.Add(x => x.CC != null);
                filter.Add(x => ccsManager.Contains(x.CC.GetValueOrDefault()));
                filter.Add(x => !string.IsNullOrEmpty(x.JobRole));

                var entityList = _iuw.UserDetailsRepository.GetList(filter);

                entityList = entityList.OrderBy(x => x.FullName);

                ViewBag.FullName = FullName;
                ViewBag.Username = Username;
                ViewBag.JobRole = JobRole;
                ViewBag.Department = Department;
                ViewBag.CC = CC;


                ViewBag.ManagerUserPermission = await entityList.ToPagedListAsync(pageNumber, itensPages);

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Consulta ao relatório ManagerUserPermission realizada.");

                return View();

            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao exibir consulta ManagerUserPermission: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }

        }

        [Authorize(Policy = "Manager")]
        public string ChangePassword(string Username)
        {
            string password = "";

            try
            {
                password = _dataWrite.ChangePassword(Username);

                if (string.IsNullOrEmpty(password) || Username == null)
                {
                    return "Erro ao trocar a senha, informa para TI o horário e usuário selecionado";
                }

                return password;

            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao desabilitar usuários: " + e.ToString());
                return "";
            }
        }
    }
}