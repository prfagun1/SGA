using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SGA.Interfaces;
using SGA.Lib;
using SGA.Models;

namespace SGA.Controllers
{
    public class ProceduresController : Controller
    {
        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "Procedures";
        private readonly IDataWrite _dataWrite;
        private readonly IDataImport _dataImport;
        private readonly IUserHelper _userHelper;
        public ProceduresController(IUnitOfWork iuw, IDataWrite dataWrite, IDataImport dataImport, IUserHelper userHelper)
        {
            _iuw = iuw;
            _dataWrite = dataWrite;
            _dataImport = dataImport;
            _userHelper = userHelper;
        }


        [Authorize(Policy = "HR")]
        public IActionResult UserCreateEmployee(DateTime? StartDate, DateTime? EndDate)
        {
            try
            {
                bool status = false;

                if (!StartDate.HasValue || !EndDate.HasValue)
                {
                    ViewBag.UserList = "";
                    return View();
                }

                status = _dataImport.ImportNewEmployees();
                //status = true;

                if (!status)
                {
                    ViewBag.Status = "Erro ao importar dados do Metadados";
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao criar usuários: Erro ao importar dados do metadados.");
                }

                var filter = new List<Expression<Func<Models.UserCreateEmployee, bool>>>() { x => x.Status == EnumSGA.UserCreateEmployeeStatus.Pendind && x.AdmissionDate >= StartDate && x.AdmissionDate < EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59) };

                var userList = _iuw.UserCreateEmployeeRepository.GetList(filter).OrderBy(x => x.FullName).ToList();
                _userHelper.CreateEmployeeUser(userList);

                ViewBag.UserList = userList;

                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Executado processo para criar usuários.");
                return View();
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao criar usuários: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }
        }

                

        [Authorize(Policy = "HR")]
        public IActionResult EnableDisableHRUsers(string button)
        {
            try{
                ViewBag.UserList = "";

                if (button == "DisableHRUsers")
                {
                    //bool status = _dataImport.ImportUserHRList();
                    bool status = true;
                    if (!status)
                    {
                        ViewBag.Status = "Erro ao importar dados do Metadados";
                        _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao criar executar processo de habilitar/desabilitar usuários: Erro ao importar dados do metadados.");
                        return View();
                    }

                    var filter = new List<Expression<Func<Models.UserHR, bool>>>() { x => x.ChangeDate >= DateTime.Now.AddMinutes(-5) };
                    _dataWrite.EnableDisableUsers();

                    IQueryable<UserHR> userHRList = _iuw.UserHRRepository.GetList(filter, include: source => source
                       .Include(a => a.UserHRApplication)
                       .ThenInclude(a => a.Application)
                    );

                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Executado processo para desabilitar usuários.");
                    ViewBag.UserList = userHRList;
                    ViewBag.Status = "Procedimento executado com sucesso";
                }

                return View();
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao desabilitar usuários: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }
        }


        [Authorize(Policy = "Administration")]
        public IActionResult ImportData()
        {
            return View();
        }

        [Authorize(Policy = "Administration")]
        public  JsonResult ImportDataService()
        {
            try
            {
                _dataImport.ImportAll();
                _dataImport.ImportUserHRList();

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Importação manual de dados realizada.");
                GC.Collect();

                return Json("Dados importados.");
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao importar dados manualmente: " + e.ToString());
                return Json("Erro ao importar dados" + e.ToString());
            }

        }


        [Authorize(Policy = "UserManagementWrite")]
        public IActionResult EnableDisableUsers()
        {
            LoadFormFields();
            return View();
        }


        [HttpPost]
        [Authorize(Policy = "UserManagementWrite")]
        public IActionResult EnableDisableUsersAction(string Username, string button)
        {
            string status = "";
            LoadFormFields();

            if (Username == null) {
                status = "É preciso selecionar um usuário";
                ViewBag.Status = status;
            }

            switch (button) {
                case "EnableUser":
                    break;
                case "DisableUser":
                     _dataWrite.EnableDisableUsers(Username);
                    break;
                case "DisableUserTemporary":
                    break;
            }

            try
            {

            }
            catch(Exception e) {
                status = "Erro ao desativar usuário";
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao desativar usuário com ID {Username}: " + e.ToString());

            }

            status = $"Usuário {Username} desativado.";

            ViewBag.Status = status;
            _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Usuário {Username} desativado.");

            return View("EnableDisableUsers");
        }

        [Authorize(Policy = "Report")]
        public JsonResult GetUserDetails(int type)
        {
            string search = "";
            string queryType;

            try
            {
                var parameter = HttpContext.Request.QueryString.ToString().Replace("%5D", "").Replace("%5B", "").Split("&");

                queryType = parameter[2].Split("=")[1];
                parameter = parameter[0].Split("=");
                search = parameter[1].Replace("%20", " ");

                var filter = new List<Expression<Func<UserDetails, bool>>>();
                filter.Add(x => x.Username.Contains(search) || x.FullName.Contains(search));

                if (queryType == "Disable") {
                }

                if (queryType == "ChangePassword") {
                    filter.AddRange(FilterChangePassword());
                }

                var userDetailsList = _iuw.UserDetailsRepository.GetList(filter).ToList();

                return Json(userDetailsList);
            }
            catch
            {
                return null;
            }
        }


        [Authorize(Policy = "Manager")]
        public IActionResult ChangePassword()
        {
            ViewBag.Username = new SelectList( "Id", "Username");
            return View();
        }

        [Authorize(Policy = "Manager")]
        public IActionResult ChangePasswordAction(string Username)
        {
            try
            {
                if (Username == null)
                {
                    ViewBag.Status = "É preciso selecionar um usuário";
                    return View();
                }

                var password = _dataWrite.ChangePassword(Username);
                if (string.IsNullOrEmpty(password))
                {
                    ViewBag.Status = "Erro ao trocar a senha, informa para TI o horário e usuário selecionado";
                    return View();
                }

                ViewBag.Status = $"Senha do usuário {Username} alterada para {password}";

                ViewBag.Username = new SelectList("Id", "Username");

                return View("ChangePassword");
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao desabilitar usuários: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        private List<Expression<Func<UserDetails, bool>>> FilterChangePassword()
        {
            var username = HttpContext.User.Identity.Name;

            var filterCC = new List<Expression<Func<CC, bool>>>();
            filterCC.Add(x => x.Username == username);
            List<int> ccsManager = _iuw.CCRepository.GetList(filterCC).Select(x => x.Id).ToList();

            var filterUserDetails = new List<Expression<Func<UserDetails, bool>>>();
            filterUserDetails.Add(x => x.CC != null);
            filterUserDetails.Add(x => ccsManager.Contains(x.CC.GetValueOrDefault()));
            filterUserDetails.Add(x => !string.IsNullOrEmpty(x.JobRole));

            return filterUserDetails;
        }

        private void LoadFormFields()
        {
            //var usernameList = _iuw.UserDetailsRepository.GetList().OrderBy(x => x.Username);
            //ViewBag.Username = new SelectList(usernameList, "Id", "Username");
            ViewBag.Username = new SelectList("Id", "Username");
        }
    }
}