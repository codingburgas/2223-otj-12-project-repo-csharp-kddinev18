using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using WebApp.BLL.Server.BLL;
using WebApp.DAL.Models;

namespace WebApp.Controllers
{
    enum OperationTypes
    {
        GetDevices = 1,
        GetData = 2,
        SentData = 3
    }

    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Devices()
        {
            CurrentUserModel currentUser = JsonConvert.DeserializeObject<CurrentUserModel>(TempData["CurrentUserInformation"].ToString());
            IEnumerable<JsonObject> model = ServerLogic.LocalServerCommunication(currentUser.Id, "{\"OperationType\":\"GetDevices\"}");
            TempData.Keep();
            return View(model);
        }

        [HttpGet]
        public IActionResult DeviceData(string deviceName)
        {
            CurrentUserModel currentUser = JsonConvert.DeserializeObject<CurrentUserModel>(TempData["CurrentUserInformation"].ToString());
            IEnumerable<JsonObject> model = ServerLogic.LocalServerCommunication(currentUser.Id, 
            "{\"OperationType\":\"GetData\", \"Arguments\" : { \"DeviceName\":\"Temperature\", \"PagingSize\":\"10\", \"SkipAmount\":\"0\"}}");
            TempData.Keep();
            return View(model);
        }
    }
}
