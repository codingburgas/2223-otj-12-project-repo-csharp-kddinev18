using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class DevicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
