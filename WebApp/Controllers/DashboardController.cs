using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using WebApp.BLL.Server.BLL;
using WebApp.BLL.Services;
using WebApp.DAL.Data;
using WebApp.DAL.Models;

namespace WebApp.Controllers
{
    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Devices()
        {
            CurrentUserModel currentUser = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");
            IEnumerable<Dictionary<string, object>> model = ServerLogic.LocalServerCommunication(currentUser.Id, "{\"OperationType\":\"GetDevices\"}");
            return View(model);
        }

        [HttpGet]
        public IActionResult DeviceData(string deviceName, string chartType, string xData, string yData, string zData, int skipAmount, int pageIndex, int pagingSize = 10)
        {
            CurrentUserModel currentUser = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");
            int count = int.Parse(ServerLogic.LocalServerCommunication(currentUser.Id,
            "{\"OperationType\":\"GetCount\", \"Arguments\" : { \"DeviceName\":\"" + deviceName + "\"}}").First()["Count"].ToString());

            DeviceDataModel deviceDataModel = new DeviceDataModel(ServerLogic.LocalServerCommunication(currentUser.Id,
            "{\"OperationType\":\"GetData\", \"Arguments\" : { \"DeviceName\":\"" + deviceName + "\", \"PagingSize\":\""+ pagingSize + "\", \"SkipAmount\":\""+ skipAmount + "\"}}"));
            deviceDataModel.Count = count;
            deviceDataModel.PageIndex = pageIndex;
            deviceDataModel.PagingSize = pagingSize;
            deviceDataModel.SkipAmount = skipAmount;
            deviceDataModel.NumberOfPages = (int)Math.Ceiling((double)count / deviceDataModel.PagingSize);

            if (chartType == "none")
                deviceDataModel.ChartType = "none";
            else
                deviceDataModel.ChartType = chartType;
            if(xData == "none")
                deviceDataModel.XData = 0;
            else
                deviceDataModel.XData = deviceDataModel.Infrastructure.IndexOf(xData);
            if (yData == "none")
                deviceDataModel.YData = 0;
            else
                deviceDataModel.YData = deviceDataModel.Infrastructure.IndexOf(yData);
            if (zData == "none")
                deviceDataModel.ZData = 0;
            else
                deviceDataModel.ZData = deviceDataModel.Infrastructure.IndexOf(zData);

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
                yData = chartData["YData"],
                zData = chartData["ZData"]
            });
        }

        public IActionResult PreviusPage(string data)
        {
            CurrentUserModel currentUser = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");
            DeviceDataModel deviceDataModel = JsonConvert.DeserializeObject<DeviceDataModel>(data);

            if (deviceDataModel.PageIndex != 0)
            {
                deviceDataModel.SkipAmount -= deviceDataModel.PagingSize;
                deviceDataModel.PageIndex--;
            }

            return RedirectToAction("DeviceData", new
            {
                deviceName = currentUser.LastSeenDevice,
                chartType = deviceDataModel.ChartType,
                xData = deviceDataModel.XData,
                yData = deviceDataModel.YData,
                zData = deviceDataModel.ZData,
                skipAmount = deviceDataModel.SkipAmount,
                pageIndex = deviceDataModel.PageIndex,
                pagingSize = deviceDataModel.PagingSize
            });
        }
        public IActionResult NextPage(string data)
        {
            CurrentUserModel currentUser = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");
            DeviceDataModel deviceDataModel = JsonConvert.DeserializeObject<DeviceDataModel>(data);

            if (deviceDataModel.NumberOfPages - 1 != deviceDataModel.PageIndex)
            {
                deviceDataModel.SkipAmount += deviceDataModel.PagingSize;
                deviceDataModel.PageIndex++;
            }

            return RedirectToAction("DeviceData", new
            {
                deviceName = currentUser.LastSeenDevice,
                chartType = deviceDataModel.ChartType,
                xData = deviceDataModel.XData,
                yData = deviceDataModel.YData,
                zData = deviceDataModel.ZData,
                skipAmount = deviceDataModel.SkipAmount,
                pageIndex = deviceDataModel.PageIndex,
                pagingSize = deviceDataModel.PagingSize
            });
        }
    }
}
