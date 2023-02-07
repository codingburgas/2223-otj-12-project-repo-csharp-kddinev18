using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class DeviceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
