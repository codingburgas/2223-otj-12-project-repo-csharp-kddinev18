using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.DataTransferObjects;
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

                return View();
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

                string token = await _authenticationService.RegisterAsync(user.UserName, user.Email, user.Password);
                HttpContext.Session.SetString("userToken", _protector.Protect(token));

                return LogIn();
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
                    return LogIn();
                }
                string token = HttpContext.Session.GetString("userToken");
                string newToken = await _authenticationService.LogInLocalServerAsync(token, user.UserName, user.Password);
                HttpContext.Session.SetString("userToken", _protector.Protect(newToken));

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return LogIn();
            }
        }
    }
}
