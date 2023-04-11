using BridgeAPI.BLL.Interfaces;
using BridgeAPI.BLL.Services.Interfaces;
using BridgeAPI.DAL.Models;
using BridgeAPI.DTO;
using BridgeAPI.DTO.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;
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
        private ILocalServerCommunicationService _localServerCommunicationService;
        public AuthenticationController(IAuthenticationService authenticationService, ITokenService tokenService, IResponseFormatterService responseFormatterService, ILocalServerCommunicationService localServerCommunicationService)
        {
            _authenticationService = authenticationService;
            _tokenService = tokenService;
            _responseFormatterService = responseFormatterService;
            _localServerCommunicationService = localServerCommunicationService;
        }

        [HttpGet("LogIn")]
        public async Task<IActionResult> LogIn()
        {
            try
            {
                string request;
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    request = await reader.ReadToEndAsync();
                }
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                IResponseDataTransferObject user =
                    await _authenticationService.LogInAsync(new UserRequestDataTransferObject()
                    {
                        UserName = jObject["UserName"].ToString(),
                        Password = jObject["Password"].ToString()
                    });
                return _responseFormatterService.FormatResponse(200, await _tokenService.GenerateToken(user), null, null);
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
        public async Task<IActionResult> LocalServerLogin()
        {
            try
            {
                string request;
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    request = await reader.ReadToEndAsync();
                }
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = await _tokenService.CeckAuthentication(jObject);

                jObject = JsonSerializer.Deserialize<JsonObject>(jObject["Arguments"].ToString());
                JsonObject localServerId = await _localServerCommunicationService.AuthenticateAsync
                (
                    userToken.TokenId,
                    jObject["UserName"].ToString(),
                    jObject["Password"].ToString()
                );
                return _responseFormatterService.FormatResponse(
                    200,
                    await _tokenService.UpdateLocalServer(
                        userToken.TokenId,
                        new Guid(localServerId["UserId"].ToString())
                    ),
                    null,
                    null
                );

            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseFormatterService.FormatResponse(401, ex.Message, ex.Message, null);
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

        [HttpPost("Register")]
        public async Task<IActionResult> Register()
        {
            try
            {
                string request;
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    request = await reader.ReadToEndAsync();
                }
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

        [HttpGet("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                string request;
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    request = await reader.ReadToEndAsync();
                }
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = await _tokenService.CeckAuthentication(jObject);
                _tokenService.DeleteToken(userToken.TokenId);
                return _responseFormatterService.FormatResponse(200, null, null, null);
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseFormatterService.FormatResponse(401, ex.Message, ex.Message, null);
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

        [HttpGet("LocalServerSignOut")]
        public async Task<IActionResult> LocalServerSignOut()
        {
            try
            {
                string request;
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    request = await reader.ReadToEndAsync();
                }
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = await _tokenService.CeckAuthentication(jObject);
                userToken.LocalServerId = Guid.Empty;

                return _responseFormatterService.FormatResponse(
                    200,
                    JsonSerializer.Serialize(userToken),
                    null,
                    null
                ); ;
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseFormatterService.FormatResponse(401, ex.Message, ex.Message, null);
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
    }
}
