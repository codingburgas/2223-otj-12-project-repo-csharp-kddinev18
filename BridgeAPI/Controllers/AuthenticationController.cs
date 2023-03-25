using BridgeAPI.BLL.Interfaces;
using BridgeAPI.DTO;
using BridgeAPI.DTO.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BridgeAPI.Controllers
{
    public class AuthenticationController : Controller
    {
        private IAuthenticationService _authenticationService;
        private ITokenService _tokenService;
        public AuthenticationController(IAuthenticationService authenticationService, ITokenService tokenService)
        {
            _authenticationService = authenticationService;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<string> Login(string request)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
            IResponseDataTransferObject user =
                await _authenticationService.LogInAsync(new UserRequestDataTransferObject()
                {
                    UserName = jObject["UserName"].ToString(),
                    Password = jObject["Password"].ToString()
                });

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
