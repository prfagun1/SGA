using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Interfaces;
using SGA.Models;

namespace SGA.Controllers
{

    public class HomeController : Controller
    {

        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "Home";
        public HomeController(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }

        [Authorize(Policy = "Public")]
        public IActionResult Index()
        {


            try
            {
                int quantidadeAplicacoes = _iuw.ApplicationRepository.Count();
                int quantidadeUsuarios = _iuw.UserDetailsRepository.Count();
                int quantidadeUsuariosPermissoesImportadas = _iuw.UserAccessRepository.Count();

                ViewBag.QuantidadeAplicacoes = quantidadeAplicacoes;
                ViewBag.QuantidadeUsuarios = quantidadeUsuarios;
                ViewBag.QuantidadeUsuariosPermissoesImportadas = quantidadeUsuariosPermissoesImportadas;

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Consulta realizada.");

                return View();
            }

            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao exibir consulta: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }
        }



        [Authorize(Policy = "Public")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
