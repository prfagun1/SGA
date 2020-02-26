using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Interfaces;
using SGA.Models.ViewModel;
using SGA.Repositories;

namespace SGA.Controllers
{
    public class AuthenticationController : Controller
    {

        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "Authentication";
        private readonly IAuthentication authentication;
        public AuthenticationController(IUnitOfWork iuw, IAuthentication authentication)
        {
            _iuw = iuw;
            this.authentication = authentication;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login, string returnUrl)
        {
            List<Claim> claims = new List<Claim>();

            if (ModelState.IsValid)
            {

                //Verifica se está sendo usado o usuário administrador, caso não esteja verifica no AD
                if (!authentication.VerifyAdminUser(login))
                {
                    try
                    {

                        claims = authentication.GetClaims(login);
                        if (claims == null || claims.Count == 0)
                        {
                            ModelState.AddModelError("", "Usuário ou senha inválida");
                            return this.View(login);
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("Username", "Erro ao acessar ao acessar o servidor de autenticação");
                        _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao logar: " + e.ToString());
                        return View();
                    }

                }
                else
                {
                    claims = Lib.AuthenticationHelper.GetClaimType(Models.EnumSGA.AccessType.Administration);
                }

                // Create the identity from the user info
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, login.Username));
                identity.AddClaim(new Claim(ClaimTypes.Name, login.Username));
                identity.AddClaims(claims);

                // Authenticate using the identity
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = login.RememberMe });

                if (returnUrl != null)
                {
                    return View(returnUrl);
                }

                _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Login realizado com sucesso do usuario {login.Username}.");

                return this.RedirectToAction("Index", "Home");
            }

            return this.View(login);

        }

        public async Task<IActionResult> LogOff()
        {
            _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Logout realizado.");

            await HttpContext.SignOutAsync();
            return this.RedirectToAction("Login", "Authentication");
        }
    }
}