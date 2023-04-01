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
        public string Register()
        {
            string protectedData = HttpContext.Session.GetString("userToken");
            return protectedData + "\n\n\n\n" + _protector.Unprotect(protectedData);
        }

        [HttpPost]
        public IActionResult Register(UserRegisterDataTransferObject user)
        {
            if (!ModelState.IsValid)
            {
                //return Register();
            }

            return View();
        }


        [HttpGet]
        public IActionResult LogInLocalServer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogInLocalServer(UserDataTransferObject user)
        {
            if (!ModelState.IsValid)
            {
                return LogInLocalServer();
            }

            return View();
        }
    }
}
