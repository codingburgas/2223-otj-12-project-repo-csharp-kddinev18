

using BridgeAPI.BLL.Interfaces;

namespace BridgeAPI.Controllers
{
    public class AuthenticationController : Controller
    {
        private IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public IActionResult Login(string request)
        {
            return View();
        }

        [HttpPost]
        public IActionResult LocalServerLogin(string request)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register()
        {
            return View();
        }
    }
}
