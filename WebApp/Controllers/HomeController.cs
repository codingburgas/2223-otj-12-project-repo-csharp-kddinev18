using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public static class TempDataExtensions
    {
        public static void Put<T>(ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static void Put(ITempDataDictionary tempData, string key, string value)
        {
            tempData[key] = value;
        }

        public static T Get<T>(ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);

            tempData.Keep();
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        public static string Get(ITempDataDictionary tempData, string key)
        {
            object o;
            tempData.TryGetValue(key, out o);

            tempData.Keep();
            try
            {
                return o == null ? null : (string)o;
            }
            catch (Exception)
            {
                return ((DateTime)o).ToString("yyyy-MM-dd");
            }
        }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetString("LastActivityTime", DateTime.Now.ToString());
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}