using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WebApp.DataTransferObjects;
using WebApp.Models;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    public class AuthenticationController : BaseController
    {
        private IAuthenticationService _authenticationService;
        private IDataProtector _protector;

        public AuthenticationController(IAuthenticationService authenticationService, IDataProtectionProvider dataProtectionProvider, IOptions<SessionOptions> sessionOptions) : base(sessionOptions)
        {
            _authenticationService = authenticationService;
            _protector = dataProtectionProvider.CreateProtector("TokenEncryption");
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(UserDataTransferObject user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return LogIn();
                }

                string token = await _authenticationService.LogInAsync(user.UserName, user.Password);
                HttpContext.Session.SetString("userToken", _protector.Protect(token));

                HttpContext.Session.SetString("LoggedUserInformation", JsonSerializer.Serialize(new LoggedUserInformation
                {
                    GlobalServer = true,
                    LocalServer = false
                }));

                return RedirectToAction("LogInLocalServer");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return LogIn();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDataTransferObject user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Register();
                }

                string token = await _authenticationService.RegisterAsync(user.UserName, user.Email, user.Password, user.Image);
                HttpContext.Session.SetString("userToken", _protector.Protect(token));

                return RedirectToAction("LogIn");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return LogIn();
            }
        }


        [HttpGet]
        public IActionResult LogInLocalServer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogInLocalServer(UserDataTransferObject user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return LogInLocalServer();
                }
                string token = _protector.Unprotect(HttpContext.Session.GetString("userToken"));
                string newToken = await _authenticationService.LogInLocalServerAsync(token, user.UserName, user.Password);
                HttpContext.Session.SetString("userToken", _protector.Protect(newToken));
                HttpContext.Session.SetString("LoggedUserInformation", JsonSerializer.Serialize(new LoggedUserInformation
                {
                    GlobalServer = true,
                    LocalServer = true
                }));

                return RedirectToAction("Index", "Devices");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Authentication", "LogIn");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return LogIn();
            }
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                string token = _protector.Unprotect(HttpContext.Session.GetString("userToken"));
                await _authenticationService.SignOut(token);
                HttpContext.Session.SetString("userToken", "");
                HttpContext.Session.SetString("LoggedUserInformation", JsonSerializer.Serialize(new LoggedUserInformation
                {
                    GlobalServer = false,
                    LocalServer = false
                }));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return LogIn();
            }
        }
    }
}
