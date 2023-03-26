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
                return _responseFormatterService.FormatResponse(200, _tokenService.GenerateToken(user), null, null);
            }
            catch (ArgumentException ex)
            {
                return _responseFormatterService.FormatResponse(400, ex.Message, ex.Message, null);
            }
            catch (JsonException)
            {
                return _responseFormatterService.FormatResponse(400, "Incorrect request", "Incorrect request", null);
            }
            catch (NullReferenceException)
            {
                return _responseFormatterService.FormatResponse(400, "Incorrect request", "Incorrect request", null);
            }
            catch (Exception ex)
            {
                return _responseFormatterService.FormatResponse(500, ex.Message, ex.Message, null);
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
                await _authenticationService.RegisterAsync(new UserRequestDataTransferObject()
                {
                    UserName = jObject["UserName"].ToString(),
                    Email = jObject["Password"].ToString(),
                    Password = jObject["Password"].ToString()
                });
                return _responseFormatterService.FormatResponse(200, null, null, null);
            }
            catch (ArgumentException ex)
            {
                return _responseFormatterService.FormatResponse(400, ex.Message, ex.Message, null);
            }
            catch (Exception ex)
            {
                return _responseFormatterService.FormatResponse(500, ex.Message, 
                    "Check if the JSON is formated correctly and that the data corresponds to the rquerments", null);
            }
        }
    }
}
