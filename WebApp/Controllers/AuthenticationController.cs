using Microsoft.AspNetCore.Mvc;
using WebApp.DataTransferObjects;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
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
                HttpContext.Session.SetString("userToken", token);

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserDataTransferObject user)
        {
            if (!ModelState.IsValid)
            {
                return Register();
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
