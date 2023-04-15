﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Drawing.Printing;
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

                if(devicesNames is null)
                {
                    //return NoDevices();
                }

                if(string.IsNullOrEmpty(deviceName))
                    deviceName = devicesNames.First();

                int entriesCount = await _devicesService.GetDeviceRowsCountAsync(token, deviceName);
                DevicesData viewModel = await _devicesService.GetDeviceDataAsync(token, deviceName, pagingSize, (pageNumber - 1) * pagingSize);
                viewModel.DeviceNames = devicesNames;

                TempDataExtensions.Put(TempData, "CurrentDevice", deviceName);
                TempDataExtensions.Put(TempData, "PageNumber", pageNumber.ToString());
                TempDataExtensions.Put(TempData, "TotalPages", ((int)Math.Ceiling((double)entriesCount / pagingSize)).ToString());
                TempDataExtensions.Put(TempData, "EntriesCount", entriesCount.ToString());
                TempDataExtensions.Put(TempData, "LastEntry", viewModel.Data.First()["Created"].ToString());
                _devicesService.FormatDates(viewModel);

                return View(viewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentNullException)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public IActionResult SetChartConfiguration(IFormCollection formData)
        {
            TempDataExtensions.Put(TempData, "CurrentDevice", formData["deviceName"]);
            TempDataExtensions.Put(TempData, "ChartType", formData["ChartType"]);
            TempDataExtensions.Put(TempData, "XData", formData["XData"]);
            TempDataExtensions.Put(TempData, "YData", formData["YData"]);
            TempDataExtensions.Put(TempData, "ZData", formData["ZData"]);

            return RedirectToAction("Index", new { deviceName = formData["deviceName"] });
        }

        [HttpPost]
        public async Task<IActionResult> PostDataToDevice()
        {
            try
            {
                string deviceName = Request.Form["DeviceName"];
                string postData = Request.Form["PostData"];
                await _devicesService.SendDataToDeviceAsync(
                    _protector.Unprotect(HttpContext.Session.GetString("userToken")),
                    deviceName,
                    postData
                );
                return RedirectToAction("DeviceData", "DevicesController", new { deviceName });

            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
