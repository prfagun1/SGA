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

    public class DatabaseSGAController : Controller
    {
        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "DatabaseSGA";
        public DatabaseSGAController(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }


        [Authorize(Policy = "Administration")]
        public IActionResult Index(bool registroApagado)
        {
            try
            {
                var entityList = _iuw.DatabaseSGARepository.GetList(null, x => x.Environment, x => x.DatabaseType);

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
        public IActionResult Create([Bind("Name,Description,DatabaseName,DatabaseServer,DatabaseUser,DatabasePassword,Port,EnvironmentId,DatabaseTypeId,ConnectionString,Enable")] DatabaseSGA entity)
        {
            try
            {
                entity = SetUserDate(entity);

                if (ModelState.IsValid)
                {

                    entity.DatabaseUser = Lib.Cipher.Encrypt(entity.DatabaseUser, entity.ChangeDate.ToString());
                    entity.DatabasePassword = Lib.Cipher.Encrypt(entity.DatabasePassword, entity.ChangeDate.ToString());

                    _iuw.DatabaseSGARepository.Create(entity);
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

                var entity = _iuw.DatabaseSGARepository.Get(x => x.Id == id,  x => x.Environment, x => x.DatabaseType);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Detalhes do ID {id} não existem.");
                    return NotFound();
                }

                LoadFormFields(entity);

                try
                {
                    string databaseUser = Lib.Cipher.Decrypt(entity.DatabaseUser, entity.ChangeDate.ToString());
                    ViewBag.DatabaseUser = databaseUser;
                }
                catch {
                    ViewBag.DatabaseUser = "";
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
        public IActionResult Edit(int id, [Bind("Id,Name,Description,DatabaseName,DatabaseServer,DatabaseUser,DatabasePassword,Port,EnvironmentId,DatabaseTypeId,ConnectionString,Enable")] DatabaseSGA entity)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    entity = SetUserDate(entity);

                    entity.DatabaseUser = Lib.Cipher.Encrypt(entity.DatabaseUser, entity.ChangeDate.ToString());
                    entity.DatabasePassword = Lib.Cipher.Encrypt(entity.DatabasePassword, entity.ChangeDate.ToString());

                    _iuw.DatabaseSGARepository.Update(entity);
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

                var entity = _iuw.DatabaseSGARepository.Get(x => x.Id == id,  x => x.Environment, x => x.DatabaseType);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Não existem detalhes do Id {id}.");
                    return NotFound();
                }

                try
                {
                    string databaseUser = Lib.Cipher.Decrypt(entity.DatabaseUser, entity.ChangeDate.ToString());
                    ViewBag.DatabaseUser = databaseUser;
                }
                catch
                {
                    ViewBag.DatabaseUser = "";
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

                var entity = _iuw.DatabaseSGARepository.Get(x => x.Id == id);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"O Id {id} não existe no banco de dados.");
                    return NotFound();
                }

                _iuw.DatabaseSGARepository.Delete(entity);
                _iuw.Save();

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Registro {entity.Name} apagado com sucesso.");

            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao apagar registro com {id}: {e.ToString()}.");
            }

            return RedirectToAction(nameof(Index));
        }



        private DatabaseSGA SetUserDate(DatabaseSGA entity)
        {
            entity.User = User.Identity.Name ?? "Nao informado";
            entity.ChangeDate = DateTime.Parse(DateTime.Now.ToString());

            return entity;
        }

        private void LoadFormFields(DatabaseSGA entity = null)
        {
            var environmentList = _iuw.EnvironmentRepository.GetList(new List<Expression<Func<Models.Environment, bool>>>() { x => x.Enable == EnumSGA.Status.Enabled }).OrderBy(x => x.Name);
            var databaseTypeList = _iuw.DatabaseTypeRepository.GetList(new List<Expression<Func<Models.DatabaseType, bool>>>()).OrderBy(x => x.Name);

            if (entity == null)
            {
                ViewBag.EnvironmentId = new SelectList(environmentList, "Id", "Name", EnumSGA.Status.Enabled);
                ViewBag.DatabaseTypeId = new SelectList(databaseTypeList, "Id", "Name", EnumSGA.Status.Enabled);
                ViewBag.EnableSelect = Lib.ControllersHelper.GetSelectList(EnumSGA.Status.Enabled);
            }
            else
            {
                ViewBag.EnvironmentId = new SelectList(environmentList, "Id", "Name", entity.EnvironmentId);
                ViewBag.DatabaseTypeId = new SelectList(databaseTypeList, "Id", "Name", entity.DatabaseTypeId);
                ViewBag.EnableSelect = Lib.ControllersHelper.GetSelectList(entity.Enable);
            }
        }

    }
}