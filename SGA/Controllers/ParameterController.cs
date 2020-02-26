using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGA.Interfaces;
using SGA.Models;

namespace SGA.Controllers
{

    public class ParameterController : Controller
    {
        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "Parameter";
        public ParameterController(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }

        [Authorize(Policy = "Administration")]
        public IActionResult Create()
        {
            try
            {
                //Somente cria caso não exista
                if (_iuw.ParameterRepository.Get(x => x.Id == 1) != null) {
                    return RedirectToAction("Details", new { id = 1 });
                }

                LoadFormFields();

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Cadastro iniciado");
                return View();
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao iniciar o cadastro: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }
        }


        [Authorize(Policy = "Administration")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("LogsRetentionTime,ItensPage,LogLevelApplication,LogErrorPath,AdminUser,AdminPassword,ValidaUsuarioURL,ValidaUsuarioJson,ValidaUsuarioUsername,ValidaUsuarioPassword,ExclusionListAfastamento")] Parameter entity)
        {
            try
            {
                entity = SetUserDate(entity);

                if (ModelState.IsValid)
                {
                    entity.AdminUser = Lib.Cipher.Encrypt(entity.AdminUser, entity.ChangeDate.ToString());
                    entity.AdminPassword = Lib.Cipher.Encrypt(entity.AdminPassword, entity.ChangeDate.ToString());

                    entity.ValidaUsuarioUsername = Lib.Cipher.Encrypt(entity.ValidaUsuarioUsername, entity.ChangeDate.ToString());
                    entity.ValidaUsuarioPassword = Lib.Cipher.Encrypt(entity.ValidaUsuarioPassword, entity.ChangeDate.ToString());

                    _iuw.ParameterRepository.Create(entity);
                    _iuw.Save();

                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Cadastro finalizado com sucesso dos parâmetros do sistema");
                    return RedirectToAction("Details", new { id = 1 });
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Erro ao salvar dados, contate o administrador informando a hora e o seu usuário.");
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar cadastro: {e.ToString()}");
            }


            ViewBag.AdminUser = Lib.Cipher.Decrypt(entity.AdminUser, entity.ChangeDate.ToString());

            LoadFormFields(entity);

            return View(entity);
        }


        [Authorize(Policy = "Administration")]
        public IActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Não é possível edição de ID nulo.");
                    return NotFound();
                }

                var entity = _iuw.ParameterRepository.Get(x => x.Id == id);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Detalhes do ID {id} não existem.");
                    return NotFound();
                }

                LoadFormFields(entity);

                ViewBag.AdminUser = Lib.Cipher.Decrypt(entity.AdminUser, entity.ChangeDate.ToString());
                ViewBag.AdminPassword = Lib.Cipher.Decrypt(entity.AdminPassword, entity.ChangeDate.ToString());

                ViewBag.ValidaUsuarioUsername = Lib.Cipher.Decrypt(entity.ValidaUsuarioUsername, entity.ChangeDate.ToString());
                ViewBag.ValidaUsuarioPassword = Lib.Cipher.Decrypt(entity.ValidaUsuarioPassword, entity.ChangeDate.ToString());

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Edição iniciada dos parâmetros do sistema.");

                return View(entity);
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao editar registro o ID {id}: {e.ToString()}.");
                return View("~/Views/Shared/Error.cshtml");
            }

        }


        [Authorize(Policy = "Administration")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,LogsRetentionTime,ItensPage,LogLevelApplication,LogErrorPath,AdminUser,AdminPassword,ValidaUsuarioURL,ValidaUsuarioJson,ValidaUsuarioUsername,ValidaUsuarioPassword,ExclusionListAfastamento")] Parameter entity)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    entity = SetUserDate(entity);
                    entity.AdminUser = Lib.Cipher.Encrypt(entity.AdminUser, entity.ChangeDate.ToString());
                    entity.AdminPassword = Lib.Cipher.Encrypt(entity.AdminPassword, entity.ChangeDate.ToString());

                    entity.ValidaUsuarioUsername = Lib.Cipher.Encrypt(entity.ValidaUsuarioUsername, entity.ChangeDate.ToString());
                    entity.ValidaUsuarioPassword = Lib.Cipher.Encrypt(entity.ValidaUsuarioPassword, entity.ChangeDate.ToString());

                    _iuw.ParameterRepository.Update(entity);
                    _iuw.Save();

                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Edição finalizada dos parâmetros do sistema.");
                    return RedirectToAction("Details", new { id = 1 });
                }

                LoadFormFields(entity);

                ViewBag.AdminUser = entity.AdminUser;

                return View(entity);
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao editar registro com ID {id}: {e.ToString()}");
                return View("~/Views/Shared/Error.cshtml");
            }

        }


        [Authorize(Policy = "Administration")]
        public IActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"O Id não foi informado.");
                    return NotFound();
                }

                var entity = _iuw.ParameterRepository.Get(x => x.Id == id);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Não existem detalhes do Id {id}.");
                    return NotFound();
                }

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Detalhes dos parâmetros acessados.");

                ViewBag.AdminUser = Lib.Cipher.Decrypt(entity.AdminUser, entity.ChangeDate.ToString());
                ViewBag.ValidaUsuarioUsername = Lib.Cipher.Decrypt(entity.ValidaUsuarioUsername, entity.ChangeDate.ToString());

                return View(entity);
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro na visualização dos detalhes do Id {id}: {e.ToString()}.");
                return View("~/Views/Shared/Error.cshtml");
            }
        }


        private Parameter SetUserDate(Parameter entity)
        {
            entity.User = User.Identity.Name ?? "Nao informado";
            entity.ChangeDate = DateTime.Parse(DateTime.Now.ToString());

            return entity;
        }

        private void LoadFormFields(Parameter entity = null)
        {

            if (entity == null)
            {
                ViewBag.LogLevelApplication = Lib.ControllersHelper.GetSelectList(EnumSGA.LogType.Info);
            }
            else
            {
                ViewBag.LogLevelApplication = Lib.ControllersHelper.GetSelectList(entity.LogLevelApplication);
            }
        }

    }
}