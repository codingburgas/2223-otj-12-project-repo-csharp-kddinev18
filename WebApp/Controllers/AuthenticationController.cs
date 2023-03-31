using Microsoft.AspNetCore.Mvc;
using WebApp.DataTransferObjects;

namespace WebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(UserDataTransferObject user)
        {
            if(!ModelState.IsValid)
            {
                return LogIn();
            }

            return LogIn();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserDataTransferObject user)
        {
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
            return View();
        }
    }
}
