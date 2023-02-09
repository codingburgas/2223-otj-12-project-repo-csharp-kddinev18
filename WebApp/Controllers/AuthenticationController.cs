using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using WebApp.BLL.Services;
using WebApp.DAL.Data;
using WebApp.DAL.Models;

namespace WebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _userAuthenticationService;
        private IOTHomeSecurityDbContext _dbContext;
        public AuthenticationController(IUserAuthenticationService userAuthenticationService)
        {
            _dbContext = new IOTHomeSecurityDbContext();
            _userAuthenticationService = userAuthenticationService;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User newUser)
        {
            _userAuthenticationService.Register(newUser, _dbContext);
            return RedirectToAction("LogIn");
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void LogIn(User user)
        {
            TempData["CurrentUserInformation"] = JsonConvert.SerializeObject(new CurrentUserModel() { Id = _userAuthenticationService.LogIn(user, _dbContext) });
            TempData.Keep();
            RedirectToAction("Devices", "Dashboard");
        }
    }
}
