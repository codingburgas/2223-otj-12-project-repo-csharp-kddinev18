using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
