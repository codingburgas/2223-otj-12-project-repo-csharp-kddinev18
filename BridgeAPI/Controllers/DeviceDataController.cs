 using BridgeAPI.BLL.Interfaces;
using BridgeAPI.BLL.Services.Interfaces;
using BridgeAPI.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BridgeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceDataController : Controller
    {
        private ITokenService _tokenService;
        private IResponseFormatterService _responseFormatterService;
        private ILocalServerCommunicationService _localServerCommunicationService;
        private IAuthenticationService _authenticationService;
        public DeviceDataController(ITokenService tokenService, IResponseFormatterService responseFormatterService, ILocalServerCommunicationService localServerCommunicationService, IAuthenticationService authenticationService)
        {
            _tokenService = tokenService;
            _responseFormatterService = responseFormatterService;
            _localServerCommunicationService = localServerCommunicationService;
            _authenticationService = authenticationService;
        }

        [HttpGet("GetDeviceData")]
        public async Task<IActionResult> GetDeviceData()
        {
            try
            {
                string request;
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    request = await reader.ReadToEndAsync();
                }
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = await _tokenService.CeckAuthentication(jObject, true);

                jObject = JsonSerializer.Deserialize<JsonObject>(jObject["Arguments"].ToString());
                return _responseFormatterService.FormatResponse(
                    200,
                    JsonSerializer.Serialize(
                    await _localServerCommunicationService.GetDeviceDataAsync(
                        userToken.TokenId,
                        jObject["DeviceName"].ToString(),
                        int.Parse(jObject["PagingSize"].ToString()),
                        int.Parse(jObject["SkipAmount"].ToString()),
                        jObject["Start"].ToString(),
                        jObject["End"].ToString()
                    )),
                    null,
                    null
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseFormatterService.FormatResponse(401, ex.Message, ex.Message, null);
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

        [HttpGet("GetDevices")]
        public async Task<IActionResult> GetDevices()
        {
            try
            {
                string request;
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    request = await reader.ReadToEndAsync();
                }
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = await _tokenService.CeckAuthentication(jObject, true);

                return _responseFormatterService.FormatResponse(
                    200,
                    JsonSerializer.Serialize(
                    await _localServerCommunicationService.GetDevicesAsync(
                        userToken.TokenId,
                        userToken.LocalServerId
                    )),
                    null,
                    null
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseFormatterService.FormatResponse(401, ex.Message, ex.Message, null);
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

        [HttpGet("GetRowsCount")]
        public async Task<IActionResult> GetRowsCount()
        {
            try
            {
                string request;
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    request = await reader.ReadToEndAsync();
                }
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = await _tokenService.CeckAuthentication(jObject, true);

                jObject = JsonSerializer.Deserialize<JsonObject>(jObject["Arguments"].ToString());
                return _responseFormatterService.FormatResponse(
                    200,
                    JsonSerializer.Serialize(
                    await _localServerCommunicationService.GetRowsCountAsync(
                        userToken.TokenId,
                        jObject["DeviceName"].ToString(),
                        jObject["Start"].ToString(),
                        jObject["End"].ToString()
                    )),
                    null,
                    null
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseFormatterService.FormatResponse(401, ex.Message, ex.Message, null);
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

        [HttpGet("SendDataToDevice")]
        public async Task<IActionResult> SendDataToDevice()
        {
            try
            {
                string request;
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    request = await reader.ReadToEndAsync();
                }
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = await _tokenService.CeckAuthentication(jObject, true);

                jObject = JsonSerializer.Deserialize<JsonObject>(jObject["Arguments"].ToString());
                return _responseFormatterService.FormatResponse(
                    200,
                    JsonSerializer.Serialize(
                    await _localServerCommunicationService.SendDataToDeviceAsync(
                        userToken.TokenId,
                        jObject["DeviceName"].ToString(),
                        jObject["Data"].ToString()
                    )),
                    null,
                    null
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseFormatterService.FormatResponse(401, ex.Message, ex.Message, null);
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

        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage()
        {
            try
            {
                string request;
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    request = await reader.ReadToEndAsync();
                }
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = await _tokenService.CeckAuthentication(jObject, true);

                return _responseFormatterService.FormatResponse(
                    200,
                    JsonSerializer.Serialize(
                    await _authenticationService.GetUserImage(
                        userToken.GlobalServerId
                    )),
                    null,
                    null
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseFormatterService.FormatResponse(401, ex.Message, ex.Message, null);
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
