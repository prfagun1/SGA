using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using SGA.Interfaces;
using SGA.Models;
using System;
using System.Linq;
using Environment = SGA.Models.Environment;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;

namespace SGA.Controllers
{

    public class LogController : Controller
    {
        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "Log";
        public LogController(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }

        [Authorize(Policy = "Log")]
        public IActionResult Index(int? page, EnumSGA.LogType? LogType, string Description, string Message, string User, string StartDate, string EndDate, string sortOrder, string sortOrderOld)
        {

            try
            {
                int itensPages = _iuw.ParameterRepository.Get(x => x.Id == 1).ItensPage;
                var pageNumber = page ?? 1;


                var filter = new List<Expression<Func<Log, bool>>>();

                if (LogType != null)
                {
                    filter.Add(x => x.LogType == LogType);
                }

                if (!string.IsNullOrEmpty(Description))
                {
                    filter.Add(x => x.Description.Contains(Description));
                }

                if (!string.IsNullOrEmpty(Message))
                {
                    filter.Add(x => x.Message.Contains(Message));
                }

                if (!string.IsNullOrEmpty(User))
                {
                    filter.Add(x => x.User.Contains(User));
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    filter.Add(x => x.ChangeDate >= Convert.ToDateTime(StartDate));
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    filter.Add(x => x.ChangeDate <= Convert.ToDateTime(EndDate));
                }

                var entityList = _iuw.LogRepository.GetList(filter: filter);


                if (sortOrder != null && sortOrderOld != null && sortOrder == sortOrderOld && !sortOrder.EndsWith("Desc") && pageNumber == 1)
                {
                    sortOrder += "Desc";
                }


                ViewBag.sortOrder = sortOrder;
                ViewBag.sortOrderOld = sortOrder;
                ViewBag.Description = Description;
                ViewBag.Message = Message;
                ViewBag.User = User;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;

                switch (sortOrder)
                {
                    case "Tipo de log":
                        entityList = entityList.OrderBy(x => x.LogType);
                        break;
                    case "Tipo de logDesc":
                        entityList = entityList.OrderByDescending(x => x.LogType);
                        break;
                    case "Descrição":
                        entityList = entityList.OrderBy(x => x.Description);
                        break;
                    case "DescriçãoDesc":
                        entityList = entityList.OrderByDescending(x => x.LogType);
                        break;
                    case "Mensagem":
                        entityList = entityList.OrderBy(x => x.Message);
                        break;
                    case "MensagemDesc":
                        entityList = entityList.OrderByDescending(x => x.Message);
                        break;
                    case "Usuário":
                        entityList = entityList.OrderBy(x => x.User);
                        break;
                    case "UsuárioDesc":
                        entityList = entityList.OrderByDescending(x => x.User);
                        break;
                    case "Data":
                        entityList = entityList.OrderBy(x => x.ChangeDate);
                        break;
                    case "DataDesc":
                        entityList = entityList.OrderByDescending(x => x.ChangeDate);
                        break;
                    default:
                        entityList = entityList.OrderByDescending(x => x.ChangeDate);
                        break;
                }

                ViewBag.Log = entityList.ToPagedList(pageNumber, itensPages);
                ViewBag.LogType = Lib.ControllersHelper.GetSelectList(LogType);

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