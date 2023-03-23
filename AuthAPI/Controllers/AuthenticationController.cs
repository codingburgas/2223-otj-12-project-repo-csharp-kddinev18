using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers
{
    public class AuthenticationController : Controller
    {
        private IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public Task<IActionResult> Register()
        {
            return View();
        }
    }
}
