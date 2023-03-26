using BridgeAPI.BLL.Interfaces;
using BridgeAPI.BLL.Services.Interfaces;
using BridgeAPI.DTO;
using BridgeAPI.DTO.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BridgeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AuthenticationController : Controller
    {
        private IAuthenticationService _authenticationService;
        private ITokenService _tokenService;
        private IResponseFormatterService _responseFormatterService;
        public AuthenticationController(IAuthenticationService authenticationService, ITokenService tokenService, IResponseFormatterService responseFormatterService)
        {
            _authenticationService = authenticationService;
            _tokenService = tokenService;
            _responseFormatterService = responseFormatterService;
        }

        [HttpGet("LogIn")]
        public async Task<string> LogIn(string request)
        {
            try
            {
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                IResponseDataTransferObject user =
                    await _authenticationService.LogInAsync(new UserRequestDataTransferObject()
                    {
                        UserName = jObject["UserName"].ToString(),
                        Password = jObject["Password"].ToString()
                    });
                _tokenService.GenerateToken(user);

                return _responseFormatterService.FormatResponse(200, user)
            }
            catch (Exception ex)
            {
                return "{\"Error\":\"ujas\"}";
            }
        }

        [HttpGet("LocalServerLogIn")]
        public IActionResult LocalServerLogin(string request)
        {
            return View();
        }

        [HttpPost("Register")]
        public async Task<string> Register(string request)
        {
            try
            {
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                if(await _authenticationService.RegisterAsync(new UserRequestDataTransferObject()
                {
                    UserName = jObject["UserName"].ToString(),
                    Email = jObject["Password"].ToString(),
                    Password = jObject["Password"].ToString()
                }))
                {
                    return "{\"Succes\":\"ne ujas\"}";
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                return "{\"Error\":\"ujas\"}";
            }
        }
    }
}
