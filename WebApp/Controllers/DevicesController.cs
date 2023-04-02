using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    public class DevicesController : BaseController
    {
        private IDevicesService _devicesService;
        private IDataProtector _protector;

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
    }
}
