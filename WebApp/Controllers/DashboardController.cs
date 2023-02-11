using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            IEnumerable<Dictionary<string, object>> model = ServerLogic.LocalServerCommunication(currentUser.Id, "{\"OperationType\":\"GetDevices\"}");
            TempData.Keep();
            return View(model);
        }

        [HttpGet]
        public IActionResult DeviceData(string deviceName, string chartType, string xData, string yData)
        {
            CurrentUserModel currentUser = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");

            DeviceDataModel deviceDataModel = new DeviceDataModel(ServerLogic.LocalServerCommunication(currentUser.Id,
            "{\"OperationType\":\"GetData\", \"Arguments\" : { \"DeviceName\":\"" + deviceName + "\", \"PagingSize\":\"10\", \"SkipAmount\":\"0\"}}"));
            if(chartType == null)
                deviceDataModel.ChartType = "table";
            else
                deviceDataModel.ChartType = chartType;
            if(xData == null)
                deviceDataModel.XData = 0;
            else
                deviceDataModel.XData = deviceDataModel.Infrastructure.IndexOf(xData);
            if (yData == null)
                deviceDataModel.YData = 1;
            else
                deviceDataModel.YData = deviceDataModel.Infrastructure.IndexOf(yData);

            currentUser.LastSeenDevice = deviceName;
            TempDataExtensions.Put(TempData, "CurrentUserInformation", currentUser);

            return View(deviceDataModel);
        }

        [HttpPost]
        public IActionResult SetChartArguments(IFormCollection chartData)
        {
            CurrentUserModel currentUser = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");


            return RedirectToAction("DeviceData", new
            {
                deviceName = currentUser.LastSeenDevice,
                chartType = chartData["ChartType"],
                xData = chartData["XData"],
                yData = chartData["YData"]
            });
        }
    }
}
