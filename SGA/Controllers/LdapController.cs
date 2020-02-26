using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Interfaces;
using SGA.Models;
using System;

namespace SGA.Controllers
{

    public class LdapController : Controller
    {
        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "Parameter";
        public LdapController(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }


        [Authorize(Policy = "Administration")]
        public IActionResult Create()
        {
            try
            {
                //Somente cria caso não exista
                if (_iuw.LdapRepository.Get(x => x.Id == 1) != null)
                {
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
        public IActionResult Create([Bind("Domain, BindUser, BindPassword, Enable")] Ldap entity)
        {
            try
            {
                entity = SetUserDate(entity);

                if (ModelState.IsValid)
                {
                    entity.BindUser = Lib.Cipher.Encrypt(entity.BindUser, entity.ChangeDate.ToString());
                    entity.BindPassword = Lib.Cipher.Encrypt(entity.BindPassword, entity.ChangeDate.ToString());

                    _iuw.LdapRepository.Create(entity);
                    _iuw.Save();

                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Cadastro finalizado com sucesso dos parâmetros LDAP do sistema");
                    return RedirectToAction("Details", new { id = 1 });
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Erro ao salvar dados, contate o administrador informando a hora e o seu usuário.");
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar cadastro: {e.ToString()}");
            }

            LoadFormFields(entity);
            ViewBag.AdminUser = Lib.Cipher.Decrypt(entity.BindUser, entity.ChangeDate.ToString());

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

                var entity = _iuw.LdapRepository.Get(x => x.Id == id);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Detalhes do ID {id} não existem.");
                    return NotFound();
                }

                LoadFormFields(entity);

                try
                {
                    ViewBag.BindUser = Lib.Cipher.Decrypt(entity.BindUser, entity.ChangeDate.ToString());
                }
                catch {
                    ViewBag.BindUser = "";
                }

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Edição iniciada dos parâmetros LDAP do sistema.");

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
        public IActionResult Edit(int id, [Bind("Id, Domain, BindUser, BindPassword, Enable")] Ldap entity)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    entity = SetUserDate(entity);
                    entity.BindUser = Lib.Cipher.Encrypt(entity.BindUser, entity.ChangeDate.ToString());
                    entity.BindPassword = Lib.Cipher.Encrypt(entity.BindPassword, entity.ChangeDate.ToString());

                    _iuw.LdapRepository.Update(entity);
                    _iuw.Save();

                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Edição finalizada dos parâmetros do sistema.");
                    return RedirectToAction("Details", new { id = 1 });
                }

                LoadFormFields(entity);

                ViewBag.BindUser = entity.BindUser;

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

                var entity = _iuw.LdapRepository.Get(x => x.Id == id);

                if (entity == null)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Não existem detalhes do Id {id}.");
                    return NotFound();
                }

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Detalhes dos parâmetros LDAP acessados.");

                try
                {
                    ViewBag.BindUser = Lib.Cipher.Decrypt(entity.BindUser, entity.ChangeDate.ToString());
                }
                catch {
                    ViewBag.BindUser = "";
                }

                return View(entity);
            }
            catch (Exception e)
            {
                _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro na visualização dos detalhes do Id {id}: {e.ToString()}.");
                return View("~/Views/Shared/Error.cshtml");
            }
        }


        private Ldap SetUserDate(Ldap entity)
        {
            entity.User = User.Identity.Name ?? "Nao informado";
            entity.ChangeDate = DateTime.Parse(DateTime.Now.ToString());

            return entity;
        }

        private void LoadFormFields(Ldap entity = null)
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