﻿using System;
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

    public class ApplicationTypeController : Controller
    {
        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "ApplicationType";
        public ApplicationTypeController(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }


        [Authorize(Policy = "Administration")]
        public IActionResult Index(bool registroApagado)
        {
            try
            {

                var entityList = _iuw.ApplicationTypeRepository.GetList();

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
        public IActionResult Create([Bind("Name,Enable")] ApplicationType entity)
        {
            try
            {
                entity = SetUserDate(entity);

                if (ModelState.IsValid)
                {
                    _iuw.ApplicationTypeRepository.Create(entity);
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

                var entity = _iuw.ApplicationTypeRepository.Get(filter: x => x.Id == id);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Detalhes do ID {id} não existem.");
                    return NotFound();
                }

                LoadFormFields(entity);

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
        public IActionResult Edit(int id, [Bind("Id,Name,Enable")] ApplicationType entity)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    entity = SetUserDate(entity);

                    _iuw.ApplicationTypeRepository.Update(entity);
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

                var entity = _iuw.ApplicationTypeRepository.Get(x => x.Id == id);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Não existem detalhes do Id {id}.");
                    return NotFound();
                }

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

                var entity = _iuw.ApplicationTypeRepository.Get(x => x.Id == id);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"O Id {id} não existe no banco de dados.");
                    return NotFound();
                }

                _iuw.ApplicationTypeRepository.Delete(entity);
                _iuw.Save();

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Registro {entity.Name} apagado com sucesso.");

            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao apagar registro com {id}: {e.ToString()}.");
            }

            return RedirectToAction(nameof(Index));
        }


        private ApplicationType SetUserDate(ApplicationType entity)
        {
            entity.User = User.Identity.Name ?? "Nao informado";
            entity.ChangeDate = DateTime.Parse(DateTime.Now.ToString());

            return entity;
        }


        private void LoadFormFields(ApplicationType entity = null)
        {

            if (entity == null)
            {
                ViewBag.EnableSelect = Lib.ControllersHelper.GetSelectList(EnumSGA.Status.Enabled);
            }
            else
            {
                ViewBag.EnableSelect = Lib.ControllersHelper.GetSelectList(entity.Enable);
            }
        }

    }
}