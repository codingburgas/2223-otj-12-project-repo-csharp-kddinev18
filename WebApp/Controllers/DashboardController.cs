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
            IEnumerable<Dictionary<string, object>> model = ServerLogic.LocalServerCommunication(currentUser.GlobalId, "{\"OperationType\":\"GetDevices\", \"Arguments\" : {\"UserId\":\""+ currentUser.LocalId + "\"}}");
            return View(model);
        }

        [HttpGet]
        public IActionResult DeviceData(string deviceName, string chartType, string xData, string yData, string zData, int skipAmount, int pageIndex, int pagingSize = 10)
        {
            CurrentUserModel currentUser = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");
            int count = int.Parse(ServerLogic.LocalServerCommunication(currentUser.GlobalId,
            "{\"OperationType\":\"GetCount\", \"Arguments\" : { \"DeviceName\":\"" + deviceName + "\"}}").First()["Count"].ToString());

            DeviceDataModel deviceDataModel = new DeviceDataModel(ServerLogic.LocalServerCommunication(currentUser.GlobalId,
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

        [HttpPost]
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
                xData = deviceDataModel.Infrastructure[deviceDataModel.XData],
                yData = deviceDataModel.Infrastructure[deviceDataModel.YData],
                zData = deviceDataModel.Infrastructure[deviceDataModel.ZData],
                skipAmount = deviceDataModel.SkipAmount,
                pageIndex = deviceDataModel.PageIndex,
                pagingSize = deviceDataModel.PagingSize
            });
        }

        [HttpPost]
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
                xData = deviceDataModel.Infrastructure[deviceDataModel.XData],
                yData = deviceDataModel.Infrastructure[deviceDataModel.YData],
                zData = deviceDataModel.Infrastructure[deviceDataModel.ZData],
                skipAmount = deviceDataModel.SkipAmount,
                pageIndex = deviceDataModel.PageIndex,
                pagingSize = deviceDataModel.PagingSize
            });
        }

        [HttpPost]
        public void SendData(IFormCollection data)
        {
            CurrentUserModel currentUser = TempDataExtensions.Get<CurrentUserModel>(TempData, "CurrentUserInformation");

            ServerLogic.LocalServerCommunication(currentUser.GlobalId,
                "{\"OperationType\":\"SendData\", \"Arguments\" : {\"DeviceName\":\""+ currentUser.LastSeenDevice + "\",\"Data\":\"" + data["data"].ToString() + "\"}}");
        }
    }
}
