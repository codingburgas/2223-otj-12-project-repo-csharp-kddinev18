using BridgeAPI.BLL.Interfaces;
using BridgeAPI.BLL.Services.Interfaces;
using BridgeAPI.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BridgeAPI.Controllers
{
    public class DeviceDataController : Controller
    {
        private ITokenService _tokenService;
        private IResponseFormatterService _responseFormatterService;
        private ILocalServerCommunicationService _localServerCommunicationService;
        public DeviceDataController(ITokenService tokenService, IResponseFormatterService responseFormatterService, ILocalServerCommunicationService localServerCommunicationService)
        {
            _tokenService = tokenService;
            _responseFormatterService = responseFormatterService;
            _localServerCommunicationService = localServerCommunicationService;
        }

        [HttpGet("GetDeviceData")]
        public async Task<string> GetDeviceData(string request)
        {
            try
            {
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = await _tokenService.CeckAuthentication(jObject);

                jObject = JsonSerializer.Deserialize<JsonObject>(jObject["Arguments"].ToString());
                return _responseFormatterService.FormatResponse(
                    200,
                    JsonSerializer.Serialize(
                    await _localServerCommunicationService.GetDeviceDataAsync(
                        userToken.TokenId,
                        jObject["DeviceName"].ToString(),
                        int.Parse(jObject["PagingSize"].ToString()),
                        int.Parse(jObject["SkipAmount"].ToString())
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
        public async Task<string> GetDevices(string request)
        {
            try
            {
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = await _tokenService.CeckAuthentication(jObject);

                jObject = JsonSerializer.Deserialize<JsonObject>(jObject["Arguments"].ToString());
                return _responseFormatterService.FormatResponse(
                    200,
                    JsonSerializer.Serialize(
                    await _localServerCommunicationService.GetDevicesAsync(
                        userToken.TokenId,
                        new Guid(jObject["UserId"].ToString())
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
