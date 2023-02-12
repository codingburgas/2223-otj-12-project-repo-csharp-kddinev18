using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using WebApp.BLL.Server.BLL;
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
            try
            {
                _userAuthenticationService.Register(newUser, _dbContext);
                return RedirectToAction("LogIn");
            }
            catch(Exception ex)
            {
                return RedirectToAction("Register");
            }
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogIn(User user)
        {
            CurrentUserModel currentUserModel = new CurrentUserModel() { GlobalId = _userAuthenticationService.LogIn(user, _dbContext), LastSeenDevice = "" };
            if (currentUserModel.GlobalId == -1)
                return RedirectToAction("LogIn");

            TempDataExtensions.Put(TempData, "CurrentUserInformation", currentUserModel);
            return RedirectToAction("SubLogIn");
        }

        [HttpGet]
        public IActionResult SubLogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubLogIn(User user)
        {
            CurrentUserModel currentUserModel = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");
            currentUserModel.LocalId = int.Parse(ServerLogic.LocalServerCommunication(currentUserModel.GlobalId,
                "{\"OperationType\" : \"Authenticate\", \"Arguments\" : {\"UserName\" : \"" + user.UserName + "\", \"Password\" : \"" + user.Password + "\"}}").First()["UserId"].ToString());

            TempDataExtensions.Put(TempData, "CurrentUserInformation", currentUserModel);
            return RedirectToAction("Devices", "Dashboard");
        }
    }
}
