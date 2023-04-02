using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Drawing.Printing;
using WebApp.DataTransferObjects;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    public class DevicesController : BaseController
    {
        private IDevicesService _devicesService;
        private IDataProtector _protector;
        private int pagingSize = 3;
        public DevicesController(IDevicesService devicesService, IDataProtectionProvider dataProtectionProvider, IOptions<SessionOptions> sessionOptions) : base(sessionOptions)
        {
            _devicesService = devicesService;
            _protector = dataProtectionProvider.CreateProtector("TokenEncryption");
        }

        public async Task<IActionResult> Index()
        {
            string token = _protector.Unprotect(HttpContext.Session.GetString("userToken"));
            return View(await _devicesService.GetDevicesAsync(token));
        }

        public async Task<IActionResult> DeviceData(string deviceName="Temperature", int pageNumber = 1)
        {
            string token = _protector.Unprotect(HttpContext.Session.GetString("userToken"));

            TempData["CurrentDevice"] = deviceName;
            TempData["PageNumber"] = pageNumber;
            TempData["TotalPages"] = (int)Math.Ceiling((double)await _devicesService.GetDeviceRowsCountAsync(token, deviceName)/pagingSize);

            return View(await _devicesService.GetDeviceDataAsync(token, deviceName, pagingSize, (pageNumber - 1) * pagingSize));
        }
    }
}
