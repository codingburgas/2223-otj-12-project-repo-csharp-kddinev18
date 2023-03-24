using Microsoft.AspNetCore.Mvc;

namespace BridgeAPI.Controllers
{
    public class DeviceDataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
