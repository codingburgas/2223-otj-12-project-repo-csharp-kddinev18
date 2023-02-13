using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return Register();
            }
            return RedirectToAction("LogIn");
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
            CurrentUserModel currentUserModel = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");
            if (currentUserModel.GlobalId != 0)
            {
                if(currentUserModel.LocalId != 0)
                {
                    return RedirectToAction("SubLogIn");
                }
                return RedirectToAction("Devices", "Dashboard");
            }

            try
            {
                currentUserModel = new CurrentUserModel() { GlobalId = _userAuthenticationService.LogIn(user, _dbContext), LastSeenDevice = "" };
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return LogIn();
            }

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
            try
            {
                IEnumerable<Dictionary<string, object>> response = ServerLogic.LocalServerCommunication(currentUserModel.GlobalId,
                "{\"OperationType\" : \"Authenticate\", \"Arguments\" : {\"UserName\" : \"" + user.UserName + "\", \"Password\" : \"" + user.Password + "\"}}");

                if (response.First().ContainsKey("Error"))
                    throw new Exception($"{response.First()["Error"]}");

                currentUserModel.LocalId = int.Parse(response.First()["UserId"].ToString());

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return LogIn();
            }
            
            TempDataExtensions.Put(TempData, "CurrentUserInformation", currentUserModel);
            return RedirectToAction("Devices", "Dashboard");
        }

        [HttpGet]
        public IActionResult SignOutFromLocalServer()
        {
            CurrentUserModel currentUserModel = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");
            currentUserModel.LocalId = 0;
            currentUserModel.LastSeenDevice = "";
            TempDataExtensions.Put(TempData, "CurrentUserInformation", currentUserModel);
            return SubLogIn();
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            CurrentUserModel currentUserModel = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");
            currentUserModel.LocalId = 0;
            currentUserModel.GlobalId = 0;
            currentUserModel.LastSeenDevice = "";
            TempDataExtensions.Put(TempData, "CurrentUserInformation", currentUserModel);
            return RedirectToAction("Index", "Home");
        }
    }
}
