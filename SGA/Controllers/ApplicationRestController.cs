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

    public class ApplicationRestController : Controller
    {
        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "ApplicationRest";
        public ApplicationRestController(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }


        [Authorize(Policy = "Administration")]
        public IActionResult Index(bool registroApagado)
        {
            try
            {

                var entityList = _iuw.ApplicationRestRepository.GetList(null, x => x.Application, x => x.ApplicationType);

                if (registroApagado)
                {
                    ViewBag.RegistroApagado = "<p>Registro apagado com sucesso </p>";
                }

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, "Consulta realizada.");

                entityList = entityList.OrderBy(x => x.Name);

                return View(entityList.ToList());
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, "Erro ao exibir consulta: " + e.ToString());
                return View("~/Views/Shared/Error.cshtml");
            }
        }


        [Authorize(Policy = "Administration")]
        public IActionResult Create()
        {
            try
            {
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
        public IActionResult Create([Bind("Name,Description,ApplicationId,ApplicationTypeId,RestType,URL,API,Header,Json,Username,Password,MD5,Enable")] ApplicationRest entity)
        {
            try
            {
                entity = SetUserDate(entity);

                if (ModelState.IsValid)
                {
                    entity.Username = Lib.Cipher.Encrypt(entity.Username, entity.ChangeDate.ToString());
                    entity.Password = Lib.Cipher.Encrypt(entity.Password, entity.ChangeDate.ToString());

                    _iuw.ApplicationRestRepository.Create(entity);
                    _iuw.Save();

                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Cadastro finalizado com sucesso do registro {entity.Name}");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Erro ao salvar dados, contate o administrador informando a hora e o seu usuário.");
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar cadastro: {e.ToString()}");
            }

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

                var entity = _iuw.ApplicationRestRepository.Get(x => x.Id == id, x => x.Application, x => x.ApplicationType);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Detalhes do ID {id} não existem.");
                    return NotFound();
                }

                LoadFormFields(entity);

                try
                {
                    ViewBag.Username = Lib.Cipher.Decrypt(entity.Username, entity.ChangeDate.ToString());
                    ViewBag.Password = Lib.Cipher.Decrypt(entity.Password, entity.ChangeDate.ToString());
                }
                catch {
                    ViewBag.Username = "";
                    ViewBag.Password = "";
                }

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Edição iniciada do registro {entity.Name}.");

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
        public IActionResult Edit(int id, [Bind("Id,Name,Description,ApplicationId,ApplicationTypeId,RestType,URL,API,Header,Json,Username,Password,MD5,Enable")] ApplicationRest entity)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    entity = SetUserDate(entity);

                    entity.Username = Lib.Cipher.Encrypt(entity.Username, entity.ChangeDate.ToString());
                    entity.Password = Lib.Cipher.Encrypt(entity.Password, entity.ChangeDate.ToString());

                    _iuw.ApplicationRestRepository.Update(entity);
                    _iuw.Save();

                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Edição finalizada do registro {entity.Name}.");
                    return RedirectToAction(nameof(Index));
                }

                LoadFormFields(entity);

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

                var entity = _iuw.ApplicationRestRepository.Get(x => x.Id == id, x => x.Application, x => x.ApplicationType);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Não existem detalhes do Id {id}.");
                    return NotFound();
                }

                ViewBag.Username = Lib.Cipher.Decrypt(entity.Username, entity.ChangeDate.ToString());
                

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Detalhes visualizados do registro {entity.Name}.");

                return View(entity);
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro na visualização dos detalhes do Id {id}: {e.ToString()}.");
                return View("~/Views/Shared/Error.cshtml");
            }
        }


        [Authorize(Policy = "Administration")]
        [HttpPost, ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"O Id não foi informado não existe.");
                    return NotFound();
                }

                var entity = _iuw.ApplicationRestRepository.Get(x => x.Id == id);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"O Id {id} não existe no banco de dados.");
                    return NotFound();
                }

                _iuw.ApplicationRestRepository.Delete(entity);
                _iuw.Save();

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Registro {entity.Name} apagado com sucesso.");

            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao apagar registro com {id}: {e.ToString()}.");
            }

            return RedirectToAction(nameof(Index));
        }


        private ApplicationRest SetUserDate(ApplicationRest entity)
        {
            entity.User = User.Identity.Name ?? "Nao informado";
            entity.ChangeDate = DateTime.Parse(DateTime.Now.ToString());

            return entity;
        }

        private void LoadFormFields(ApplicationRest entity = null)
        {
            var applicationList = _iuw.ApplicationRepository.GetList(new List<Expression<Func<Models.Application, bool>>>() { x => x.Enable == EnumSGA.Status.Enabled }).OrderBy(x => x.Name);
            var applicationTypeList = _iuw.ApplicationTypeRepository.GetList(new List<Expression<Func<Models.ApplicationType, bool>>>() {
                x => x.Enable == EnumSGA.Status.Enabled,
                x => x.Id != (int)EnumSGA.ConnectionType.ListagemDetalhesUsuarios})
            .OrderBy(x => x.Name);

            if (entity == null)
            {
                ViewBag.ApplicationId = new SelectList(applicationList, "Id", "Name");
                ViewBag.ApplicationTypeId = new SelectList(applicationTypeList, "Id", "Name");
                ViewBag.EnableSelect = Lib.ControllersHelper.GetSelectList(EnumSGA.Status.Enabled);
                ViewBag.RestType = new SelectList(new[] { new { ID = 0, Name = "GET" }, new { ID = 1, Name = "POST" }, }, "ID", "Name");
                ViewBag.MD5 = new SelectList(new[] { new { ID = true, Name = "Sim" }, new { ID = false, Name = "Não" }, }, "ID", "Name");
            }
            else
            {
                ViewBag.ApplicationId = new SelectList(applicationList, "Id", "Name", entity.ApplicationId);
                ViewBag.ApplicationTypeId = new SelectList(applicationTypeList, "Id", "Name", entity.ApplicationType);
                ViewBag.EnableSelect = Lib.ControllersHelper.GetSelectList(entity.Enable);
                ViewBag.RestType = new SelectList(new[] { new { ID = 0, Name = "GET" }, new { ID = 1, Name = "POST" }, }, "ID", "Name", entity.RestType);
                ViewBag.MD5 = new SelectList(new[] { new { ID = true, Name = "Sim" }, new { ID = false, Name = "Não" }, }, "ID", "Name", entity.MD5);
            }
        }

    }
}