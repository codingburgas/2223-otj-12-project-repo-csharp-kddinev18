using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Drawing.Printing;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using WebApp.Models;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    public class DevicesController : BaseController
    {
        private IDevicesService _devicesService;
        private IDataProtector _protector;
        private int pagingSize = 10;
        public DevicesController(IDevicesService devicesService, IDataProtectionProvider dataProtectionProvider, IOptions<SessionOptions> sessionOptions) : base(sessionOptions)
        {
            _devicesService = devicesService;
            _protector = dataProtectionProvider.CreateProtector("TokenEncryption");
        }

        public async Task<IActionResult> Index(string deviceName = "", int pageNumber = 1)
        {
            try
            {
                string token = _protector.Unprotect(HttpContext.Session.GetString("userToken"));

                IEnumerable<string> devicesNames = await _devicesService.GetDevicesAsync(token);

                if(devicesNames.Count() == 0)
                {
                    return RedirectToAction("NoDevices");
                }

                if(string.IsNullOrEmpty(deviceName))
                    deviceName = devicesNames.First();

                DateTime start = TempDataExtensions.Get(TempData, "Start") == null ? DateTime.Now.AddDays(-7) : DateTime.Parse(TempDataExtensions.Get(TempData, "Start"));
                DateTime end = TempDataExtensions.Get(TempData, "End") == null ? DateTime.Now.AddDays(1) : DateTime.Parse(TempDataExtensions.Get(TempData, "End")).AddDays(1);

                int entriesCount = await _devicesService.GetDeviceRowsCountAsync(token, deviceName, start, end);
                DevicesData viewModel = await _devicesService.GetDeviceDataAsync(token, deviceName, pagingSize, (pageNumber - 1) * pagingSize, start, end);
                viewModel.DeviceNames = devicesNames;

                TempDataExtensions.Put(TempData, "CurrentDevice", deviceName);
                TempDataExtensions.Put(TempData, "PageNumber", pageNumber.ToString());
                TempDataExtensions.Put(TempData, "TotalPages", ((int)Math.Ceiling((double)entriesCount / pagingSize)).ToString());
                TempDataExtensions.Put(TempData, "EntriesCount", entriesCount.ToString());
                JsonObject jObject = viewModel.Data.FirstOrDefault();

                if(jObject != null)
                {
                    TempDataExtensions.Put(TempData, "LastEntry", viewModel.Data.FirstOrDefault()["Created"].ToString());
                }
                else
                {
                    TempDataExtensions.Put(TempData, "LastEntry", "");
                }

                _devicesService.FormatDates(viewModel);

                return View(viewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Authentication", "LogIn");

            }
            catch (ArgumentNullException)
            {
                return Unauthorized();
            }
        }

        public IActionResult NoDevices()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetChartConfiguration(IFormCollection formData)
        {
            TempDataExtensions.Put(TempData, "CurrentDevice", formData["deviceName"]);
            TempDataExtensions.Put(TempData, "ChartType", formData["ChartType"]);
            TempDataExtensions.Put(TempData, "XData", formData["XData"]);
            TempDataExtensions.Put(TempData, "YData", formData["YData"]);
            TempDataExtensions.Put(TempData, "ZData", formData["ZData"]);
            TempDataExtensions.Put(TempData, "Start", formData["Start"]);
            TempDataExtensions.Put(TempData, "End", formData["End"]);

            return RedirectToAction("Index", new { deviceName = formData["deviceName"] });
        }

        [HttpPost]
        public async Task<IActionResult> PostDataToDevice(IFormCollection formData)
        {
            string deviceName = "";
            try
            {
                deviceName = TempDataExtensions.Get(TempData, "CurrentDevice"); 
                string postData = formData["DeviceData"];
                await _devicesService.SendDataToDeviceAsync(
                    _protector.Unprotect(HttpContext.Session.GetString("userToken")),
                    deviceName,
                    postData
                );
                return RedirectToAction("Index", new { deviceName });

            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("LogIn", "Authentication");

            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { deviceName });
            }
        }
    }
}
