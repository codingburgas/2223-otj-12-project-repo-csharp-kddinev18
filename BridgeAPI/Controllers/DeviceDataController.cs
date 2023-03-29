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
        private IAuthenticationService _authenticationService;
        private ITokenService _tokenService;
        private IResponseFormatterService _responseFormatterService;
        private ILocalServerCommunicationService _localServerCommunicationService;
        public DeviceDataController(IAuthenticationService authenticationService, ITokenService tokenService, IResponseFormatterService responseFormatterService, ILocalServerCommunicationService localServerCommunicationService)
        {
            _authenticationService = authenticationService;
            _tokenService = tokenService;
            _responseFormatterService = responseFormatterService;
            _localServerCommunicationService = localServerCommunicationService;
        }
        public async Task<string> GetDeviceData(string request)
        {
            try
            {
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
                Token userToken = JsonSerializer.Deserialize<Token>(jObject["Token"].ToString());
                Token serverToken = await _tokenService.GetToken(userToken.TokenId);
                if (serverToken is null)
                {
                    throw new UnauthorizedAccessException("Not authenticated");
                }
                if (serverToken.ExpireDate < DateTime.Now)
                {
                    throw new UnauthorizedAccessException("Token is expired");
                }
                if (serverToken.SecretKey != userToken.SecretKey)
                {
                    throw new UnauthorizedAccessException("Not authenticated");
                }
                return _responseFormatterService.FormatResponse(
                    200,
                    _localServerCommunicationService.GetDeviceDataAsync(jObject["Request"].ToString()),
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
