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

namespace SGA.Controllers
{
    public class UserHRController : Controller
    {

        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "UserHR";
        public UserHRController(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }

        [Authorize(Policy = "UserManagementRead")]
        public async Task<IActionResult> Index(
            int? page, string Username, string FullName,
            string DateHRStart, string DateHREnd, string DateHRReturnStart, string DateHRReturnEnd,
            string JobRole, string Department, int? CC, int? ApplicationId,
            EnumSGA.UserHRStatusLocal? UserHRStatusLocal,  EnumSGA.UserHRStatusHR? UserHRStatusHR,
            string sortOrder, string sortOrderOld
        )
        {
            try
            {
                int itensPages = _iuw.ParameterRepository.Get(x => x.Id == 1).ItensPage;
                var pageNumber = page ?? 1;


                var filter = new List<Expression<Func<UserHR, bool>>>();


                if (!string.IsNullOrEmpty(FullName))
                {
                    filter.Add(x => x.FullName.Contains(FullName));
                }

                if (!string.IsNullOrEmpty(Username))
                {
                    filter.Add(x => x.Username.Contains(Username));
                }

                if (DateHRStart != null)
                {
                    filter.Add(x => x.MetadadosDate >= Convert.ToDateTime(DateHRStart));
                }

                if (DateHREnd != null)
                {
                    filter.Add(x => x.MetadadosDate <= Convert.ToDateTime(DateHREnd).AddHours(23).AddMinutes(59).AddSeconds(59));
                }

                if (DateHRReturnStart != null)
                {
                    filter.Add(x => x.MetadadosDateReturn >= Convert.ToDateTime(DateHRReturnStart));
                }

                if (DateHRReturnEnd != null)
                {
                    filter.Add(x => x.MetadadosDateReturn <= Convert.ToDateTime(DateHRReturnEnd).AddHours(23).AddMinutes(59).AddSeconds(59));
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

                if (ApplicationId != null)
                {
                   filter.Add(x => x.UserHRApplication.Any(y => y.ApplicationId == ApplicationId) );
                }

                if (UserHRStatusLocal != null)
                {
                    filter.Add(x => x.StatusLocal == UserHRStatusLocal);
                }

                if (UserHRStatusHR != null)
                {
                    filter.Add(x => x.StatusRH == UserHRStatusHR);
                }


                var entityList = _iuw.UserHRRepository.GetList(filter, include: source => source
                    .Include(a => a.UserHRApplication)
                    .ThenInclude(a => a.Application)
                );


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
                    case "Data desligamento":
                        entityList = entityList.OrderBy(x => x.MetadadosDate) ;
                        break;
                    case "Data desligamentoDesc":
                        entityList = entityList.OrderByDescending(x => x.MetadadosDate);
                        break;
                    case "Data Volta":
                        entityList = entityList.OrderBy(x => x.MetadadosDateReturn);
                        break;
                    case "Data VoltaDesc":
                        entityList = entityList.OrderByDescending(x => x.MetadadosDateReturn);
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
                    case "Status RH":
                        entityList = entityList.OrderBy(x => x.StatusRH);
                        break;
                    case "Status RHDesc":
                        entityList = entityList.OrderByDescending(x => x.StatusRH);
                        break;
                    case "Status Local":
                        entityList = entityList.OrderBy(x => x.StatusLocal);
                        break;
                    case "Status LocalDesc":
                        entityList = entityList.OrderByDescending(x => x.StatusLocal);
                        break;
                    default:
                        entityList = entityList.OrderByDescending(x => x.MetadadosDate).ThenBy(x => x.FullName);
                        break;
                }


                ViewBag.Username = Username;
                ViewBag.FullName = FullName;
                ViewBag.DateHRStart = DateHRStart ;
                ViewBag.DateHREnd = DateHREnd;
                ViewBag.DateHRReturnStart = DateHRReturnStart;
                ViewBag.DateHRReturnEnd = DateHRReturnEnd;
                ViewBag.JobRole = JobRole;
                ViewBag.Department = Department;
                ViewBag.CC = CC;

                ViewBag.UserHRStatusLocal = Lib.ControllersHelper.GetSelectList(UserHRStatusLocal);
                ViewBag.UserHRStatusHR = Lib.ControllersHelper.GetSelectList(UserHRStatusHR);

                var applicationList = _iuw.ApplicationRepository.GetList(new List<Expression<Func<Models.Application, bool>>>()).OrderBy(x => x.Name);
                ViewBag.ApplicationId = new SelectList(applicationList, "Id", "Name");
                ViewBag.UserHR = await entityList.ToPagedListAsync(pageNumber, itensPages);

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Consulta realizada.");

                return View();

            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao exibir consulta: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }

        }


    }
}


