using GlobalServer.BLL.Server.BLL;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.DAL.Models;

namespace WebApp.Controllers
{
    enum OperationTypes
    {
        GetDevices = 1,
        GetData = 2,
        SentData = 3
    }

    public class DeviceController : Controller
    {
        [HttpGet]
        public IActionResult Devices()
        {
            CurrentUserModel currentUser = JsonConvert.DeserializeObject<CurrentUserModel>(TempData["CurrentUserInformation"].ToString());
            return View(
                ServerLogic.LocalServerCommunication(currentUser.Id,
                "{\"OperationType\":\"GetDevices\"}"));
        }
    }
}
