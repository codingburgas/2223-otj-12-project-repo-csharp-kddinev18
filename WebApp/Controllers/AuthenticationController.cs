using Microsoft.AspNetCore.Mvc;
using WebApp.BLL;
using WebApp.DAL;
using WebApp.DAL.Data.Models;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private IOTHomeSecurityDbContext _dbContext;
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User newUser)
        {
            UserAuthentication.Register(newUser, _dbContext);
            return LogIn();
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(User newUser)
        {
            return View();
        }
    }
}
