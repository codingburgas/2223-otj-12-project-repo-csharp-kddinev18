using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult LogIn()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult LogInLocalServer()
        {
            return View();
        }
    }
}
